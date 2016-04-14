using System.Diagnostics.Contracts;

namespace Since.Rdf
{
    /// <summary>
    /// 
    /// </summary>
    [Immutable]
    public class Edge : IMatchable<Edge>
    {
        /// <summary>
        /// Initializes a new edge, representing an RDF triple or quad.
        /// </summary>
        /// <remarks>All nodes, depending on situation, can additionally be of types: <see cref="AnyNode"/>.</remarks>
        /// <param name="subject">The subject node. Expected to be a resource node. See <see cref="BlankNode"/>, <see cref="IriNode"/>.</param>
        /// <param name="predicate">The predicate node. Expected to be an IRI node. See <see cref="IriNode"/>.</param>
        /// <param name="obj">The object node. Can be of any RDF node type.</param>
        /// <param name="context">The context node. Expected to be a resource node. See <see cref="BlankNode"/>, <see cref="IriNode"/>. If <see cref="null"/>, a new <see cref="DefaultContextNode"/> is used.</param>
        public Edge(INode subject, INode predicate, INode obj, INode context = null)
        {
            Contract.Requires(subject != null);
            Contract.Requires(predicate != null);
            Contract.Requires(obj != null);

            this.Subject = subject;
            this.Predicate = predicate;
            this.Object = obj;
            this.Context = context ?? new DefaultContextNode();
        }

        /// <summary>
        /// The subject node.
        /// </summary>
        public INode Subject { get; }

        /// <summary>
        /// The predicate node.
        /// </summary>
        public INode Predicate { get; }

        /// <summary>
        /// The object node.
        /// </summary>
        public INode Object { get; }

        /// <summary>
        /// The context node.
        /// </summary>
        public INode Context { get; }

        /// <summary>
        /// Gets a new edge with any nodes being replaced by non-null parameters.
        /// </summary>
        /// <param name="subject">A subject node, or <see langword="null"/>.</param>
        /// <param name="predicate">A predicate node, or <see langword="null"/>.</param>
        /// <param name="obj">An object node, or <see langword="null"/>.</param>
        /// <param name="context">A contet node, or <see langword="null"/>.</param>
        /// <returns>A new edge.</returns>
        public Edge With(INode subject = null, INode predicate = null, INode obj = null, INode context = null)
            => new Edge(
                subject ?? this.Subject,
                predicate ?? this.Predicate,
                obj ?? this.Object,
                context ?? this.Context);

        /// <inheritDoc />
        public bool Matches(Edge edge)
            => Edge.Matches(this, edge);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3} .", this.Subject?.ToString() ?? "null",
                this.Predicate?.ToString() ?? "null", this.Object?.ToString() ?? "null",
                this.Context?.ToString() ?? "null");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="edgeA"></param>
        /// <param name="edgeB"></param>
        /// <returns></returns>
        public static bool Matches(Edge edgeA, Edge edgeB)
        {
            if (edgeA == null || edgeB == null)
                return edgeA == edgeB;

            return edgeA.Subject.Matches(edgeB.Subject)
                   && edgeA.Predicate.Matches(edgeB.Predicate)
                   && edgeA.Object.Matches(edgeB.Object)
                   && edgeA.Context.Matches(edgeB.Context);
        }
    }

    public static class EdgeExtensions
    {
        public static bool Matches(this Edge edge, INode subject = null, INode predicate = null, INode obj = null, INode context = null)
            => edge.Matches(new Edge(subject, predicate, obj, context));
    }
}