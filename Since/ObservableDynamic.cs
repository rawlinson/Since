using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Dynamic;

namespace Since
{
    /// <summary>
    ///     Represents an observable object whose members can be dynamically added and removed by dictionary at run time.
    /// </summary>
    /// <typeparam name="TDict">The type of dictionary.</typeparam>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    public abstract class ObservableDynamic<TDict, TKey, TValue>
        : DynamicObject, INotifyPropertyChanged
        where TDict : INotifyCollectionChanged, IDictionary<TKey, TValue>
    {
        protected readonly TDict _items;

        /// <summary>
        ///     Initializes a new <see cref="ObservableDynamic{TDict, TKey, TValue}" /> using the specified dictionary.
        /// </summary>
        /// <param name="items">The observable dictionary that contains the keys and values.</param>
        protected ObservableDynamic(TDict items)
        {
            Contract.Requires(items != null);

            _items = items;
            _items.CollectionChanged += this.Items_CollectionChanged;
        }

        /// <summary>
        ///     Gets a value indicating whether the <see cref="ObservableDynamic{TDict, TKey, TValue}" /> is read-only.
        /// </summary>
        /// <value>
        ///     <see langword="true" /> if the <see cref="ObservableDynamic{TDict, TKey, TValue}" /> is read-only; otherwise,
        ///     <see langword="false" />.
        /// </value>
        public bool IsReadOnly
            => _items.IsReadOnly;

        /// <summary>
        ///     Occurs when a property value changes.
        /// </summary>
        /// <remarks>
        ///     The event can indicate all properties on the object have changed by using either <see langword="null" /> or
        ///     <see cref="string.Empty">String.Empty</see> as the property name in the <see cref="PropertyChangedEventArgs" />.
        /// </remarks>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Gets the key for the specified member name.
        /// </summary>
        /// <param name="name">The member name whose key to get.</param>
        /// <param name="result">
        ///     When this method returns, the member name associated with the specified key, if the key is found;
        ///     otherwise, the default value for the type of the key parameter. This parameter is passed uninitialized.
        /// </param>
        /// <returns><see langword="true" /> if a key is found for the member name; otherwise, <see langword="false" />.</returns>
        protected abstract bool TryGetKey(string name, out TKey result);

        /// <summary>
        ///     Gets the member name for the specified key.
        /// </summary>
        /// <param name="key">The key whose member name to get.</param>
        /// <param name="result">
        ///     When this method returns, the key associated with the specified member name, if the name is found;
        ///     otherwise, <see langword="null" />. This parameter is passed uninitialized.
        /// </param>
        /// <returns><see langword="true" /> if a member name is found for the key; otherwise, <see langword="false" />.</returns>
        protected abstract bool TryGetName(TKey key, out string result);

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Contract.Requires(e != null);

            IEnumerable items;            

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    items = e.NewItems;
                    break;
                case NotifyCollectionChangedAction.Remove:
                    items = e.OldItems;
                    break;
                case NotifyCollectionChangedAction.Replace:
                    throw new NotImplementedException("TODO!");
                default:
                    throw new NotImplementedException($"{e.Action} should not be possible");
            }

            foreach (var item in items)
            {
                string name;
                if (this.TryGetName(((KeyValuePair<TKey, TValue>) item).Key, out name))
                    this.OnPropertyChanged(name);
            }
        }

        private void OnPropertyChanged(string propertyName = null)
            => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        /// <summary>
        ///     Provides the implementation for operations that get member values.
        /// </summary>
        /// <param name="binder">Provides information about the object that called the dynamic operation.</param>
        /// <param name="result">The result of the get operation.</param>
        /// <returns>
        ///     <see langword="true" />
        /// </returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            Contract.Requires(binder != null);

            TValue value;

            TKey key;
            if (this.TryGetKey(binder.Name, out key))
                _items.TryGetValue(key, out value);
            else
                value = default(TValue);

            result = value;
            return true; // Always return true - the member may be added later.
        }

        /// <summary>
        ///     Provides the implementation for operations that set member values.
        /// </summary>
        /// <param name="binder">Provides information about the object that called the dynamic operation.</param>
        /// <param name="value">The value to set to the member.</param>
        /// <returns>
        ///     <see langword="true" /> if the operation is successful; otherwise, <see langword="false" />.
        /// </returns>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            Contract.Requires(binder != null);
            Contract.Requires(value is TValue);
            Contract.Requires(!this.IsReadOnly);

            TKey key;
            var exists = this.TryGetKey(binder.Name, out key);
            if (exists)
                _items[key] = (TValue) value;

            return exists;
        }
    }
}