using Since.Rdf;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Since.Odm
{
    public class Subject
    {
        private readonly List<Edge> _edges;
        private readonly MultiValueDictionary<INode, Edge> _predicates;

        public Subject(INode about, INode type = null)
        {
            Contract.Requires(about != null);

            _edges = new List<Edge>();
            _predicates = new MultiValueDictionary<INode, Edge>();

            this.About = about;
            this.Type = type;
            if (this.Type != null)
                this.AddInternal(new Edge(this.About, Rdf.Schema.Rdf.Type, this.Type));
        }

        public INode About { get; }

        public INode Type { get; }

        public IEnumerable<Edge> Edges
            => _edges;

        public IEnumerable<INode> this[INode predicate]
            => _predicates[predicate].Select(edge => edge.Object);

        public void Add(Edge edge)
        {
            Contract.Requires(edge != null);
            Contract.Requires(edge.Predicate != null);
            Contract.Requires(edge.Object != null);
            
            if (edge.Subject == null)
                edge = edge.With(subject: this.About);

            this.AddInternal(edge);
        }

        public void Add(INode predicate, INode obj, INode context = null)
            => this.AddInternal(new Edge(this.About, predicate, obj, context));
        
        public void AddOrReplace(Edge edge)
        {

        }
        public void AddOrReplace(INode predicate, INode obj, INode context = null)
            => this.AddInternal(new Edge(this.About, predicate, obj, context));

        private void AddInternal(Edge edge)
        {
            _edges.Add(edge);
            if (edge.Subject.Equals(this.About))
                _predicates.Add(edge.Predicate, edge);
        }
    }
}