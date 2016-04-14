namespace Since.Rdf
{
    /// <summary>
    /// The interface of all nodes.
    /// </summary>
    [Immutable]
    public interface INode : IMatchable<INode>
    {
    }

    /// <summary>
    /// The base class for nodes.
    /// </summary>
    [Immutable]
    public abstract class Node : INode
    {
        /// <inheritdoc />
        public bool Matches(INode other)
            => Node.Matches(this, other);

        /// <summary>
        /// Matches the supplied nodes.
        /// </summary>
        /// <seealso cref="IMatchable{T}"/>
        /// <param name="nodeA">A node.</param>
        /// <param name="nodeB">Another node.</param>
        /// <returns><see langword="false"/> if the nodes matches; otherwise; <see langword="false"/>.</returns>
        public static bool Matches(INode nodeA, INode nodeB)
            => nodeA is AnyNode || nodeB is AnyNode || (nodeA?.Equals(nodeB) ?? nodeA == nodeB);
    }

    /// <summary>
    /// A node that can match any other node.
    /// </summary>
    [Immutable]
    public class AnyNode : Node
    {
        /// <inheritdoc />
        public override string ToString()
        {
            return "%any";
        }
    }

    /// <summary>
    /// A blank node.
    /// </summary>
    [Immutable]
    public class BlankNode : Node
    {
        /// <inheritdoc />
        public override string ToString()
        {
            return "_:" + this.GetHashCode();
        }
    }

    /// <summary>
    /// The default context node.
    /// </summary>
    /// <remarks>Represents the default context of the graph it is added to.</remarks>
    [Immutable]
    public class DefaultContextNode : Node
    {
        /// <inheritdoc />
        public override string ToString()
        {
            return "%default";
        }
    }
}
