using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Since.Rdf
{
    /// <summary>
    /// 
    /// </summary>
    public class DataSet : DataSetBase
    {
        private readonly IEdgeSet _edges;

        private readonly Collections.HashSet<INode> _nodes;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="edgeSet"></param>
        public DataSet(IEdgeSet edgeSet = null)
        {
            _edges = edgeSet ?? new IndexedEdgeSet(IndexCoverage.Default);
            _nodes = new Collections.HashSet<INode>();
        }

        /// <inheritdoc />
        public override IEnumerable<Edge> Edges => _edges;

        /// <inheritdoc />
        public override int EdgeCount => _edges.Count;

        /// <inheritdoc />
        public override IEnumerable<INode> Nodes => _nodes;

        /// <inheritdoc />
        public override int NodeCount => _nodes.Count;
        
        private INode GetOrAddNode(INode node)
        {
            _nodes.GetOrAdd(ref node);
            return node;
        }

        /// <inheritdoc />
        public override void Add(Edge edge)
        {
            Contract.Requires(edge != null);
            Contract.Requires(edge.Subject != null);
            Contract.Requires(edge.Predicate != null);
            Contract.Requires(edge.Object != null);

            edge.Subject = this.GetOrAddNode(edge.Subject);
            edge.Predicate = this.GetOrAddNode(edge.Predicate);
            edge.Object = this.GetOrAddNode(edge.Object);
            edge.Context = edge.Context == null ? this.DefaultGraphNode : this.GetOrAddNode(edge.Context);

            _edges.GetOrAdd(ref edge);
        }

        /// <inheritdoc />
        public override void AddRange(IEnumerable<Edge> edges)
        {
            Contract.Requires(edges != null);

            var collection = edges as ICollection<Edge>;
            if (collection != null)
                _edges.Capacity = _edges.Count + collection.Count;

            foreach (var edge in edges)
                this.Add(edge);
        }

        /// <inheritdoc />
        public override IEnumerable<Edge> AllWhere(Edge edge)
        {
            throw new NotImplementedException();
        }
    }
}
