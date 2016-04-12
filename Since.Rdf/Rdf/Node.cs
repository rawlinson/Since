using System;

namespace Since.Rdf
{
    /// <summary>
    /// 
    /// </summary>
    [Immutable]
    public interface INode : IMatchable<INode>
    {
    }

    /// <summary>
    /// 
    /// </summary>
    [Immutable]
    public abstract class Node : INode
    {
        /// <inheritdoc />
        public bool Matches(INode other)
            => Node.Matches(this, other);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeA"></param>
        /// <param name="nodeB"></param>
        /// <returns></returns>
        public static bool Matches(INode nodeA, INode nodeB)
            => nodeA is AnyNode || nodeB is AnyNode || nodeA.Equals(nodeB);
    }

    /// <inheritdoc />
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
    /// 
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
}