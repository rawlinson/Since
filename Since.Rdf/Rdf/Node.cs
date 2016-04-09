using System;

namespace Since.Rdf
{
    /// <summary>
    /// 
    /// </summary>
    public interface INode : IMatchable<INode>
    {
    }

    /// <summary>
    /// 
    /// </summary>
    public abstract class Node : INode
    {
        /// <inheritdoc />
        public bool Matches(INode other)
            => Node.Matches(this, other);

        private static bool Matches<T>(T nodeA, T nodeB)
            where T : INode
            => nodeA.Equals(nodeB);

        private static bool Matches<T>(AnyNode _, T __)
            where T : INode
            => true;

        private static bool Matches<T>(T _, AnyNode __)
            where T : INode
            => true;
    }

    /// <inheritdoc />
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
    public class BlankNode : Node
    {
        /// <inheritdoc />
        public override string ToString()
        {
            return "_:" + this.GetHashCode();
        }
    }
}