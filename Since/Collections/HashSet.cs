using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Since.Collections
{
    /// <summary>
    ///     Represents a strongly typed set of objects.
    /// </summary>
    /// <typeparam name="T">The type of elements in the set.</typeparam>
    public class HashSet<T> : ICollection<T>, IReadOnlyCollection<T>
    {
        private const int Lower31BitMask = 0x7FFFFFFF;
        private const int ShrinkThreshold = 3;
        private const int MaxPrimeArrayLength = 0x7FEFFFFD;

        private int[] _buckets;
        private int _freeList;
        private int _lastIndex;
        private Slot[] _slots;
        private int _version;

        /// <summary>
        ///     Initializes a new instance of the <see cref="HashSet{T}" /> class that is empty
        ///     and optionally uses the specified <see cref="IEqualityComparer{T}" />.
        /// </summary>
        /// <param name="comparer">
        ///     The <see cref="IEqualityComparer{T}" /> implementation to use when comparing items, or <see langword="null" /> to
        ///     use
        ///     the default <see cref="IEqualityComparer{T}" /> for the type of the set. Default is <see langword="null" />.
        /// </param>
        public HashSet(IEqualityComparer<T> comparer = null)
        {
            _lastIndex = 0;
            _freeList = -1;
            _version = 0;

            this.Comparer = comparer ?? EqualityComparer<T>.Default;
            this.Count = 0;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="HashSet{T}" /> class that is empty, has the specified suggested
        ///     capacity,
        ///     and optionally uses the specified <see cref="IEqualityComparer{T}" />.
        /// </summary>
        /// <param name="suggestedCapacity">
        ///     The suggested number of elements that the new set can store. See
        ///     <see cref="Capacity" />.
        /// </param>
        /// <param name="comparer">
        ///     The <see cref="IEqualityComparer{T}" /> implementation to use when comparing items, or <see langword="null" /> to
        ///     use
        ///     the default <see cref="IEqualityComparer{T}" /> for the type of the set. Default is <see langword="null" />.
        /// </param>
        public HashSet(int suggestedCapacity, IEqualityComparer<T> comparer = null)
            : this(comparer)
        {
            Contract.Requires(suggestedCapacity > 0);

            this.InitializeCapacity(suggestedCapacity);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="HashSet{T}" /> class that contains elements copied from the specified
        ///     collection,
        ///     has sufficient capacity to accommodate the number of elements copied, and optionally uses the specified
        ///     <see cref="IEqualityComparer{T}" />.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new list.</param>
        /// <param name="comparer">
        ///     The <see cref="IEqualityComparer{T}" /> implementation to use when comparing items, or <see langword="null" /> to
        ///     use
        ///     the default <see cref="IEqualityComparer{T}" /> for the type of the set. Default is <see langword="null" />.
        /// </param>
        public HashSet(IEnumerable<T> collection, IEqualityComparer<T> comparer = null)
            : this(comparer)
        {
            Contract.Requires(collection != null);

            var suggestedCapacity = 0;
            var coll = collection as ICollection<T>;
            if (coll != null)
                suggestedCapacity = coll.Count;

            this.InitializeCapacity(suggestedCapacity);

            this.UnionWith(collection);
            if ((this.Count == 0 && _slots.Length > Prime.Primes[0]) ||
                (this.Count > 0 && _slots.Length/this.Count > HashSet<T>.ShrinkThreshold))
                this.TrimExcess();
        }

        /// <summary>
        ///     Gets the <see cref="IEqualityComparer{T}" /> object that is used to determine equality for the values in the
        ///     <see cref="HashSet{T}" />.
        /// </summary>
        public IEqualityComparer<T> Comparer { get; }

        /// <summary>
        ///     Gets the element that matches the specified item.
        /// </summary>
        /// <param name="item"></param>
        /// <exception cref="KeyNotFoundException"><paramref name="item" /> is not found in the set.</exception>
        /// <value>The element in the set that matches the specified item.</value>
        public T this[T item]
        {
            get
            {
                if (!this.TryGetItem(ref item))
                    throw new KeyNotFoundException();
                return item;
            }
        }

        /// <summary>
        ///     Gets or sets the total number of elements the internal data structure can hold without resizing.
        /// </summary>
        public int Capacity
        {
            set
            {
                var size = Prime.GetNextPrime(value);
                if (size < this.Count)
                    return;
                this.SetCapacity(size, false);
            }
            get { return _slots?.Length ?? 0; }
        }

        /// <summary>
        ///     Gets the number of elements that are contained in the <see cref="HashSet{T}" />.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether the <see cref="HashSet{T}" /> is read-only.
        /// </summary>
        public bool IsReadOnly
            => false;

        void ICollection<T>.Add(T item)
            => this.Add(item);

        /// <summary>
        ///     Removes the specified item from the <see cref="HashSet{T}" />.
        /// </summary>
        /// <param name="item"></param>
        /// <returns><see langword="true" /> if the item is found; otherwise, <see langword="false" />.</returns>
        public bool Remove(T item)
        {
            if (_buckets == null)
                return false;

            var hashCode = this.GetItemHashCode(item);
            var bucket = hashCode%_buckets.Length;
            var last = -1;
            for (var i = _buckets[bucket] - 1; i >= 0; last = i, i = _slots[i].Next)
            {
                if (_slots[i].HashCode == hashCode && this.Comparer.Equals(_slots[i].Value, item))
                {
                    if (last < 0)
                        _buckets[bucket] = _slots[i].Next + 1;
                    else
                        _slots[last].Next = _slots[i].Next;

                    _slots[i].HashCode = -1;
                    _slots[i].Value = default(T);
                    _slots[i].Next = _freeList;

                    this.Count--;
                    _version++;
                    if (this.Count == 0)
                    {
                        _lastIndex = 0;
                        _freeList = -1;
                    }
                    else
                        _freeList = i;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     Determines whether the <see cref="HashSet{T}" /> contains a specific value.
        /// </summary>
        /// <param name="item"></param>
        /// <returns><see langword="true" /> if the set contains the specified item; otherwise, <see langword="false" />.</returns>
        public bool Contains(T item)
        {
            return this.TryGetItem(ref item);
        }

        /// <summary>
        ///     Removes all items from the <see cref="HashSet{T}" />.
        /// </summary>
        public void Clear()
        {
            if (_lastIndex > 0)
            {
                Array.Clear(_slots, 0, _lastIndex);
                Array.Clear(_buckets, 0, _buckets.Length);
                _lastIndex = 0;
                _freeList = -1;
                this.Count = 0;
            }
            _version++;
        }

        /// <summary>
        ///     Returns an enumerator that iterates through the <see cref="HashSet{T}" />.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
            => new Enumerator(this);

        IEnumerator IEnumerable.GetEnumerator()
            => this.GetEnumerator();


        public void CopyTo(T[] array, int arrayIndex)
            => this.CopyTo(array, arrayIndex, this.Count);

        /// <summary>
        ///     Adds the specified element to the <see cref="HashSet{T}" />.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>
        ///     <see langword="true" /> if the element is added to the <see cref="HashSet{T}" /> object;
        ///     <see langword="false" /> if the element is already present.
        /// </returns>
        public bool Add(T item)
            => this.GetOrAdd(ref item);

        /// <summary>
        ///     Look for a specific item in the set. If found, reports the corresponding element,
        ///     otherwise adds an element with the supplied item.
        /// </summary>
        /// <param name="item">On entry, the item to add if not found. On exit, the element added or found.</param>
        /// <returns><see langword="true" /> if the item is found; otherwise, <see langword="false" />.</returns>
        public bool GetOrAdd(ref T item)
        {
            if (_buckets == null)
                this.InitializeCapacity(0);
            Contract.Assert(_buckets != null);

            var hashCode = this.GetItemHashCode(item);

            // Check if the item is present.
            for (var i = _buckets[hashCode%_buckets.Length] - 1; i >= 0; i = _slots[i].Next)
            {
                if (_slots[i].HashCode != hashCode || !this.Comparer.Equals(_slots[i].Value, item))
                    continue;
                item = _slots[i].Value;
                return true;
            }

            var bucket = hashCode%_buckets.Length;

            int index;
            if (_freeList >= 0)
            {
                index = _freeList;
                _freeList = _slots[index].Next;
            }
            else
            {
                if (_lastIndex == _slots.Length)
                {
                    this.IncreaseCapacity();
                    bucket = hashCode%_buckets.Length;
                }
                index = _lastIndex;
                _lastIndex++;
            }

            _slots[index].HashCode = hashCode;
            _slots[index].Value = item;
            _slots[index].Next = _buckets[bucket] - 1;

            _buckets[bucket] = index + 1;
            _version++;
            this.Count++;

            item = _slots[index].Value;
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool TryGetItem(ref T item)
        {
            if (_buckets == null)
                return false;

            var hashCode = this.GetItemHashCode(item);

            for (var i = _buckets[hashCode%_buckets.Length] - 1; i >= 0; i = _slots[i].Next)
            {
                if (_slots[i].HashCode != hashCode || !this.Comparer.Equals(_slots[i].Value, item))
                    continue;
                item = _slots[i].Value;
                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        /// <param name="count"></param>
        public void CopyTo(T[] array, int arrayIndex, int count)
        {
            Contract.Requires(array != null);
            Contract.Requires(arrayIndex >= 0);
            Contract.Requires(count >= 0);
            Contract.Requires(arrayIndex + count < array.Length);

            var numCopied = 0;
            for (var i = 0; i < _lastIndex && numCopied < count; i++)
            {
                if (_slots[i].HashCode < 0)
                    continue;
                array[arrayIndex + numCopied] = _slots[i].Value;
                numCopied++;
            }
        }

        private int GetItemHashCode(T item)
        {
            if (item == null)
                return 0;
            return this.Comparer.GetHashCode(item) & HashSet<T>.Lower31BitMask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public void UnionWith(IEnumerable<T> other)
        {
            Contract.Requires(other != null);

            foreach (var item in other)
                this.Add(item);
        }

        private void InitializeCapacity(int capacity)
        {
            var size = Prime.GetNextPrime(capacity);

            _buckets = new int[size];
            _slots = new Slot[size];
        }

        private void IncreaseCapacity()
        {
            var newSize = 2*this.Count;
            if (newSize > HashSet<T>.MaxPrimeArrayLength && HashSet<T>.MaxPrimeArrayLength > this.Count)
                newSize = HashSet<T>.MaxPrimeArrayLength;

            newSize = Prime.GetNextPrime(newSize);
            if (newSize <= this.Count)
                throw new ArgumentException("Cannot increase capacity");

            this.SetCapacity(newSize, false);
        }

        private void SetCapacity(int newSize, bool forceNewHashCodes)
        {
            var newSlots = new Slot[newSize];
            if (_slots != null)
                Array.Copy(_slots, 0, newSlots, 0, _lastIndex);

            if (forceNewHashCodes)
            {
                for (var i = 0; i < _lastIndex; i++)
                {
                    if (newSlots[i].HashCode != -1)
                        newSlots[i].HashCode = this.GetItemHashCode(newSlots[i].Value);
                }
            }

            var newBuckets = new int[newSize];
            for (var i = 0; i < _lastIndex; i++)
            {
                var bucket = newSlots[i].HashCode%newSize;
                newSlots[i].Next = newBuckets[bucket] - 1;
                newBuckets[bucket] = i + 1;
            }

            _slots = newSlots;
            _buckets = newBuckets;
        }

        /// <summary>
        ///     Sets the capacity of a <see cref="HashSet{T}" /> object to the actual number of elements it contains, rounded up to
        ///     a nearby, implementation-specific value.
        /// </summary>
        public void TrimExcess()
        {
            if (this.Count == 0)
            {
                // if count is zero, clear references
                _buckets = null;
                _slots = null;
                _version++;
            }
            else
            {
                // similar to IncreaseCapacity but moves down elements in case add/remove/etc
                // caused fragmentation
                var newSize = Prime.GetNextPrime(this.Count);
                var newSlots = new Slot[newSize];
                var newBuckets = new int[newSize];

                // move down slots and rehash at the same time. newIndex keeps track of current 
                // position in newSlots array
                var newIndex = 0;
                for (var i = 0; i < _lastIndex; i++)
                {
                    if (_slots[i].HashCode < 0)
                        continue;

                    newSlots[newIndex] = _slots[i];

                    // rehash
                    var bucket = newSlots[newIndex].HashCode%newSize;
                    newSlots[newIndex].Next = newBuckets[bucket] - 1;
                    newBuckets[bucket] = newIndex + 1;

                    newIndex++;
                }

                _lastIndex = newIndex;
                _slots = newSlots;
                _buckets = newBuckets;
                _freeList = -1;
            }
        }

        private struct Enumerator : IEnumerator<T>
        {
            private readonly HashSet<T> _set;
            private readonly int _version;

            private int _index;

            internal Enumerator(HashSet<T> set)
            {
                _set = set;
                _index = 0;
                _version = set._version;
                this.Current = default(T);
            }

            public bool MoveNext()
            {
                Contract.Requires(_version == _set._version);

                while (_index < _set._lastIndex)
                {
                    if (_set._slots[_index].HashCode >= 0)
                    {
                        this.Current = _set._slots[_index].Value;
                        _index++;
                        return true;
                    }
                    _index++;
                }
                _index = _set._lastIndex + 1;
                this.Current = default(T);

                return false;
            }

            public T Current { get; private set; }

            object IEnumerator.Current
                => this.Current;

            void IEnumerator.Reset()
            {
                Contract.Requires(_version == _set._version);

                _index = 0;
                this.Current = default(T);
            }

            public void Dispose() {}
        }

        internal struct Slot
        {
            public int HashCode; // Lower 31 bits of hash code, -1 if unused
            public T Value;
            public int Next; // Index of next entry, -1 if last
        }
    }
}