using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Since.Collections;

namespace Since.Rdf
{
    /// <summary>
    /// </summary>
    public class IndexedEdgeSet : IEdgeSet
    {
        private readonly Collections.HashSet<Edge> _edges = new Collections.HashSet<Edge>();
        private readonly IEnumerable<IndexCoverage> _indexes;
        private readonly MultiValueDictionary<IndexKey, Edge> _index = new MultiValueDictionary<IndexKey, Edge>();

        /// <summary>
        /// </summary>
        /// <param name="indexCoverage"></param>
        public IndexedEdgeSet(IndexCoverage indexCoverage = IndexCoverage.Basic)
        {
            this.IndexCoverage = indexCoverage;
            _indexes = Enum.GetValues(typeof (IndexCoverage))
                .OfType<IndexCoverage>()
                .Where(c => this.IndexCoverage.HasFlag(c) && c.HasSingleFlag());
        }

        /// <summary>
        /// </summary>
        public IndexCoverage IndexCoverage { get; }

        /// <summary>
        /// Gets the number of edges in the set.
        /// </summary>
        /// <value>The number of edges in the set.</value>
        public int Count
            => _edges.Count;

        /// <summary>
        /// Gets whether the set is readonly or not.
        /// </summary>
        /// <value><see langword="true"/> if the set is read only; otherwise, <see langword="false"/></value>
        public bool IsReadOnly
            => _edges.IsReadOnly;

        /// <inheritdoc />
        public int Capacity
        {
            get { return _edges.Capacity; }
            set { _edges.Capacity = value; }
        }

        /// <inheritdoc />
        public bool GetOrAdd(ref Edge edge)
        {
            if (_edges.GetOrAdd(ref edge))
                return true;

            this.AddToIndex(edge);
            return false;
        }

        /// <inheritdoc />
        public void Add(Edge edge)
            => this.GetOrAdd(ref edge);

        /// <inheritdoc />
        public void AddRange(IEnumerable<Edge> edges)
        {
            if (edges is ICollection<Edge> c)
                _edges.Capacity = _edges.Count + c.Count;

            foreach (var triple in edges)
                this.Add(triple);
        }

        /// <inheritdoc />
        public bool Remove(Edge triple)
        {
            if (!_edges.Remove(triple))
                return false;
            this.RemoveFromIndex(triple);
            return true;
        }

        /// <inheritdoc />
        public void Clear()
        {
            _edges.Clear();
            _index.Clear();
        }

        /// <inheritdoc />
        public IEnumerable<Edge> AllWhere(Edge edge)
        {
            var index = this.IndexCoverage.SelectIndex(edge);

            IEnumerable<Edge> result;
            if (index.Item1 == IndexCoverage.None)
                result = _edges;
            else
            {
                IReadOnlyCollection<Edge> r;
                if (_index.TryGetValue(IndexKey.Create(edge, index.Item1.ToMask()), out r))
                    result = r;
                else
                    result = Array.Empty<Edge>();
            }

            if (index.Item2 != TripleMask.None)
                result = result.Where(t => edge.EqualsWithMask(t, index.Item2));

            return result;
        }
        
        /// <inheritdoc />
        public IEnumerable<Edge> AllWhere(INode s = null, INode p = null, INode o = null, INode c = null)
            => this.AllWhere(new Edge(s, p, o, c));

        /// <inheritdoc />
        public bool Contains(Edge item)
            => _edges.Contains(item);

        /// <inheritdoc />
        public void CopyTo(Edge[] array, int arrayIndex)
            => _edges.CopyTo(array, arrayIndex);

        /// <inheritdoc />
        public IEnumerator<Edge> GetEnumerator()
            => _edges.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => _edges.GetEnumerator();

        private void AddToIndex(Edge triple)
        {
            Contract.Requires(triple != null);

            foreach (var index in _indexes)
                _index.Add(IndexKey.Create(triple, index.ToMask()), triple);
        }

        /// <summary>
        /// </summary>
        /// <param name="triple"></param>
        private void RemoveFromIndex(Edge triple)
        {
            Contract.Requires(triple != null);

            foreach (var index in _indexes)
                _index.Remove(IndexKey.Create(triple, index.ToMask()), triple);
        }

        private class IndexKey : IEquatable<IndexKey>
        {
            private readonly INode _context;
            private readonly INode _object;
            private readonly INode _predicate;
            private readonly INode _subject;

            private IndexKey(INode s, INode p, INode o, INode c)
            {
                _subject = s;
                _predicate = p;
                _object = o;
                _context = c;
            }

            public bool Equals(IndexKey obj)
            {
                if (obj == null)
                    return false;

                return object.Equals(_subject, obj._subject)
                    && object.Equals(_predicate, obj._predicate)
                    && object.Equals(_object, obj._object)
                    && object.Equals(_context, obj._context);
            }

            public override bool Equals(object obj)
                => this.Equals(obj as IndexKey);

            public override int GetHashCode()
            {
                unchecked // Overflow is fine, just wrap
                {
                    var hash = 17;

                    hash = hash*92821 + _subject?.GetHashCode() ?? 0;
                    hash = hash*92821 + _predicate?.GetHashCode() ?? 0;
                    hash = hash*92821 + _object?.GetHashCode() ?? 0;
                    hash = hash*92821 + _context?.GetHashCode() ?? 0;

                    return hash;
                }
            }

            public static IndexKey Create(Edge triple, TripleMask index)
            {
                return new IndexKey
                    (
                    index.HasFlag(IndexCoverage.Subject) ? triple.Subject : null,
                    index.HasFlag(IndexCoverage.Predicate) ? triple.Predicate : null,
                    index.HasFlag(IndexCoverage.Object) ? triple.Object : null,
                    index.HasFlag(IndexCoverage.Context) ? triple.Context : null
                    );
            }
        }
    }
}