﻿using System;
using System.Collections.Generic;

namespace Since.Text
{
    using NodeReference = WeakReference<InternedStringNode>;

    /// <seealso cref="InternedString" />
    public class InternedStringNode
    {
        /// <summary>
        /// The children of the node.
        /// </summary>
        public List<NodeReference> Children;

        /// <summary>
        /// The parent of the node.
        /// </summary>
        public InternedStringNode Parent;

        /// <summary>
        /// The value of the node.
        /// </summary>
        public string Value;

        /// <summary>
        /// Gets the root of the node.
        /// </summary>
        public InternedStringNode Root
        {
            get
            {
                var node = this;
                while (node.Parent != null)
                    node = node.Parent;
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
        /// <returns></returns>
        public override string ToString()
            => Value;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ToAbsoluteString()
        {
            if (Parent != null)
                return Parent.ToAbsoluteString() + Value;
            return Value;
        }

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
                if (node.Children != null)
                {
                    foreach (var wn in node.Children)
                    {
                        InternedStringNode n;
                        if (!wn.TryGetTarget(out n))
                            continue;

                        var k = 0;
                        for (; k < n.Value.Length && i < value.Length; k++, i++)
                        {
                            if (value[i] != n.Value[k])
                                break;
                        }
                        if (k == n.Value.Length)
                        {
                            next = n;
                            break;
                        }

                        if (k <= 0)
                            continue;

                        var newNode = new InternedStringNode
                        {
                            Value = n.Value.Substring(0, k),
                            Children = new List<NodeReference>(2)
                        };
                        n.Value = n.Value.Substring(k);

                        node.Children.Remove(wn);
                        newNode.Children.Add(wn);
                        n.Parent = newNode;

                        node.Children.Add(new NodeReference(node));
                        newNode.Parent = node;

                        next = newNode;
                        break;
                    }
                }

                if (next == null && i < value.Length)
                {
                    var newNode = new InternedStringNode {Parent = node, Value = value.Substring(i)};
                    if (node.Children == null)
                        node.Children = new List<NodeReference>(2);
                    node.Children.Add(new NodeReference(newNode));
                    node = newNode;
                    break;
                }

                node = next;
                if (node == null)
                    break;
            }

            return node;
        }
    }
}