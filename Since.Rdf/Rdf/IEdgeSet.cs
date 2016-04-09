using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Since.Rdf
{
    /// <summary>
    /// </summary>
    [ContractClass(typeof(EdgeSetContracts))]
    public interface IEdgeSet
        : ICollection<Edge>, IEdgeQueryable
    {
        /// <summary>
        /// </summary>
        int Capacity { get; set; }

        /// <summary>
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        bool GetOrAdd(ref Edge edge);

        /// <summary>
        /// </summary>
        /// <param name="edges"></param>
        void AddRange(IEnumerable<Edge> edges);
    }

    [ContractClassFor(typeof(IEdgeSet))]
    public abstract class EdgeSetContracts : IEdgeSet
    {
        public abstract int Capacity { get; set; }
        public abstract int Count { get; }
        public abstract bool IsReadOnly { get; }

        public abstract void Add(Edge item);
        public void AddRange(IEnumerable<Edge> edges)
        {
            Contract.Requires(edges != null);
            throw new NotImplementedException();
        }

        public abstract IEnumerable<Edge> AllWhere(Edge edge);

        public abstract IEnumerable<Edge> AllWhere(INode subject = null, INode predicate = null, INode obj = null, INode context = null);

        public abstract void Clear();
        public abstract bool Contains(Edge item);
        public abstract void CopyTo(Edge[] array, int arrayIndex);
        public abstract IEnumerator<Edge> GetEnumerator();

        public bool GetOrAdd(ref Edge edge)
        { 
            Contract.Requires(edge != null);
            throw new NotImplementedException();
        }

        public abstract bool Remove(Edge item);

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}