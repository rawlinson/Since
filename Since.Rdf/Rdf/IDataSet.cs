using System;
using System.Collections.Generic;
using Since.Net;

namespace Since.Rdf
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDataSet : IEdgeQueryable
    {
        /// <summary>
        /// 
        /// </summary>
        IriNode DefaultGraphNode { get; }

        /// <summary>
        /// 
        /// </summary>
        int NodeCount { get; }

        /// <summary>
        /// 
        /// </summary>
        IEnumerable<INode> Nodes { get; }

        /// <summary>
        /// 
        /// </summary>
        int EdgeCount { get; }

        /// <summary>
        /// 
        /// </summary>
        IEnumerable<Edge> Edges { get; }
        
        /// <summary>
        /// 
        /// </summary>
        NamespaceMapper Namespaces { get; }

        /// <summary>
        /// 
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="edge"></param>
        void Add(Edge edge);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="edges"></param>
        void AddRange(IEnumerable<Edge> edges);   
    }

    /// <summary>
    /// 
    /// </summary>
    public abstract class DataSetBase : IDataSet
    {
        /// <inheritdoc />
        public IriNode DefaultGraphNode { get; protected set; }

        /// <inheritdoc />
        public abstract int EdgeCount { get; }

        /// <inheritdoc />
        public abstract int NodeCount { get; }

        /// <inheritdoc />
        public abstract IEnumerable<INode> Nodes { get; }

        /// <inheritdoc />
        public abstract IEnumerable<Edge> Edges { get; }
        
        /// <inheritdoc />
        public virtual bool IsEmpty
            => this.EdgeCount == 0;

        /// <inheritdoc />
        public NamespaceMapper Namespaces { get; } = new NamespaceMapper();

        /// <inheritdoc />
        public abstract void Add(Edge edge);

        /// <inheritdoc />
        public virtual void AddRange(IEnumerable<Edge> edges)
        {
            foreach (var edge in edges)
                this.Add(edge);
        }

        /// <inheritdoc />
        public virtual IEnumerable<Edge> AllWhere(INode subject = null, INode predicate = null,
            INode obj = null, INode context = null)
            => AllWhere(new Edge(subject, predicate, obj, context));

        /// <inheritdoc />
        public abstract IEnumerable<Edge> AllWhere(Edge edge);
    }
}