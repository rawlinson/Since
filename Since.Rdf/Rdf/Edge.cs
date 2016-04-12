using System.ComponentModel;
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
        /// 
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="predicate"></param>
        /// <param name="obj"></param>
        /// <param name="context"></param>
        public Edge(INode subject = null, INode predicate = null, INode obj = null, INode context = null)
        {
            this.Subject = subject;
            this.Predicate = predicate;
            this.Object = obj;
            this.Context = context;
        }

        /// <summary>
        /// 
        /// </summary>
        public INode Subject { get; }

        /// <summary>
        /// 
        /// </summary>
        public INode Predicate { get; }

        /// <summary>
        /// 
        /// </summary>
        public INode Object { get; }

        /// <summary>
        /// 
        /// </summary>
        public INode Context { get; }

        /// <summary>
        /// Gets whether the <see cref="Edge"/> is empty or not.
        /// </summary>
        /// <value><see langword="true"/> if the edge is empty; otherwise, <see langword="false"/>.</value>
        public bool IsEmpty
            => this.Subject == null
            && this.Predicate == null
            && this.Object == null
            && this.Context == null;

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

        private static bool Matches(INode a, INode b)
            => a == null || b == null || a.Equals(b);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="edgeA"></param>
        /// <param name="edgeB"></param>
        /// <returns></returns>
        public static bool Matches(Edge edgeA, Edge edgeB)
        {
            Contract.Requires(edgeA != null);
            Contract.Requires(edgeB != null);

            return Edge.Matches(edgeA.Subject, edgeB.Subject)
                   && Edge.Matches(edgeA.Predicate, edgeB.Predicate)
                   && Edge.Matches(edgeA.Object, edgeB.Object)
                   && Edge.Matches(edgeA.Context, edgeB.Context);
        }
    }

    public static class EdgeExtensions
    {
        public static bool Matches(this Edge edge, INode subject = null, INode predicate = null, INode obj = null, INode context = null)
            => edge.Matches(new Edge(subject, predicate, obj, context));
    }
}