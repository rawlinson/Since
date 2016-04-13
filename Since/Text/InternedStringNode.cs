using System;
using System.Collections.Generic;

namespace Since.Text
{
    using NodeReference = WeakReference<InternedStringNode>;

    /// <seealso cref="InternedString" />
    [Immutable]
    public sealed class InternedStringNode
    {
        [Immutable(Trusted = true)]
        private InternedStringNode _parent;

        [Immutable(Trusted = true)]
        private List<NodeReference> _children;

        [Immutable(Trusted = true)]
        private string _value;

        public InternedStringNode() { }

        private InternedStringNode(InternedStringNode parent, List<NodeReference> children, string value)
        {
            _parent = parent;
            _children = children;
            _value = value;
        }

        /// <summary>
        /// The parent of the node.
        /// </summary>
        public InternedStringNode Parent
            => _parent;
        
        public bool HasParent(InternedStringNode node)
        {
            for (var n = this; n != null; n = n._parent)
            {
                if (n == node)
                    return true;
            }

            return node == null;
        }

        /// <summary>
        /// Gets the root of the node.
        /// </summary>
        public InternedStringNode Root
        {
            get
            {
                var node = this;
                while (node._parent != null)
                    node = node._parent;
                return node;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public InternedStringNode this[string value]
            => this.Get(value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public InternedStringNode Get(string value)
        {
            if (string.IsNullOrEmpty(value))
                return this;

            var node = this;

            for (var i = 0; i < value.Length;)
            {
                InternedStringNode next = null;
                if (node._children != null)
                {
                    foreach (var wn in node._children)
                    {
                        InternedStringNode n;
                        if (!wn.TryGetTarget(out n))
                            continue;

                        var k = 0;
                        for (; k < n._value.Length && i < value.Length; k++, i++)
                        {
                            if (value[i] != n._value[k])
                                break;
                        }
                        if (k == n._value.Length)
                        {
                            next = n;
                            break;
                        }

                        if (k <= 0)
                            continue;

                        var newNode = new InternedStringNode(null,
                            new List<NodeReference>(2),
                            _value.Substring(0, k));
                        n._value = n._value.Substring(k);

                        node._children.Remove(wn);
                        newNode._children.Add(wn);
                        n._parent = newNode;

                        node._children.Add(new NodeReference(node));
                        newNode._parent = node;

                        next = newNode;
                        break;
                    }
                }

                if (next == null && i < value.Length)
                {
                    var newNode = new InternedStringNode(node, null, value.Substring(i));
                    if (node._children == null)
                        node._children = new List<NodeReference>(2);
                    node._children.Add(new NodeReference(newNode));
                    node = newNode;
                    break;
                }

                node = next;
                if (node == null)
                    break;
            }

            return node;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => this.ToAbsoluteString();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ToAbsoluteString()
            => Substring(null, this);

        public static string Substring(InternedStringNode exclusiveFrom, InternedStringNode inclusiveUntil)
        {
            if (inclusiveUntil != exclusiveFrom)
                return Substring(exclusiveFrom, inclusiveUntil._parent) + inclusiveUntil._value;
            return inclusiveUntil._value;
        }
    }
}