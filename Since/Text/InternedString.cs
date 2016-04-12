using System;
using System.Diagnostics.Contracts;

namespace Since.Text
{
    /// <summary>
    ///     Represents an interned string using a <see href="https://en.wikipedia.org/wiki/Radix_tree">radix tree</see> as a
    ///     backing store.
    /// </summary>
    /// <seealso cref="InternedStringNode" />
    [Immutable]
    public class InternedString : IEquatable<InternedString>
    {
        /// <summary>
        ///     Initializes a new <see cref="InternedString" /> from the specified <see cref="System.String" />.
        /// </summary>
        /// <param name="s">The value of the new <see cref="InternedString" />.</param>
        /// <remarks>The <see cref="DefaultRoot">default</see>&#160;<see cref="InternedStringNode" /> is used as the root.</remarks>
        public InternedString(string s)
            : this(s, InternedString.DefaultRoot) {}

        /// <summary>
        ///     Initializes a new <see cref="InternedString" /> from the specified <see cref="System.String" />
        ///     using the supplied <see cref="InternedStringNode" /> as the root.
        /// </summary>
        /// <param name="s">The value of the new <see cref="InternedString" />.</param>
        /// <param name="root">The root of the new <see cref="InternedString" />.</param>
        public InternedString(string s, InternedStringNode root)
        {
            Contract.Requires(root != null);

            this.Node = root[s];
            this.Root = root;
        }

        /// <summary>
        ///     Initializes a new <see cref="InternedString" /> from the specified <see cref="InternedStringNode" />.
        /// </summary>
        /// <param name="node">The <see cref="InternedStringNode" /> of the new <see cref="InternedString" />.</param>
        internal InternedString(InternedStringNode node)
        {
            Contract.Requires(node != null);
            Contract.Requires(node.Root == this.Root);

            this.Node = node;
        }

        /// <summary>
        ///     Gets the default <see cref="InternedStringNode" /> root of <see cref="InternedString" />s.
        /// </summary>
        public static InternedStringNode DefaultRoot { get; } = new InternedStringNode();

        /// <summary>
        ///     Gets the <see cref="InternedStringNode" /> root of the <see cref="InternedString" />.
        /// </summary>
        public InternedStringNode Root { get; }

        /// <summary>
        ///     Gets the <see cref="InternedStringNode" /> of the <see cref="InternedString" />.
        /// </summary>
        public InternedStringNode Node { get; }

        /// <summary>
        ///     Determines whether the specified <see cref="InternedString" /> is equal to the current.
        /// </summary>
        /// <param name="other">The <see cref="InternedString" /> to compare with.</param>
        /// <returns>
        ///     <see langword="true" /> if the specified <see cref="InternedString" /> is equal to the current; otherwise,
        ///     <see langword="true" />.
        /// </returns>
        public bool Equals(InternedString other)
        {
            if (other == null)
                return false;
            if (object.ReferenceEquals(this, other))
                return true;
            return this.Node == other.Node;
        }

        public override bool Equals(object other)
            => this.Equals(other as InternedString);

        /// <summary>
        ///     Determines whether the beginning of this <see cref="InternedString" /> instance matches the specified
        ///     <see cref="InternedString" />.
        /// </summary>
        /// <param name="value">The <see cref="InternedString" /> to compare with.</param>
        /// <returns>
        ///     <see langword="true" /> if <paramref name="value" /> matches the beginning of this string; otherwise,
        ///     <see langword="true" />
        /// </returns>
        public bool StartsWith(InternedString value)
        {
            Contract.Requires(value != null);

            for (var node = this.Node; node != null; node = node.Parent)
            {
                if (node == value.Node)
                    return true;
            }

            return false;
        }

        public override int GetHashCode()
            => new {this.Node}.GetHashCode();

        public override string ToString()
            => this.Node.ToAbsoluteString();

        /// <summary>
        ///     Appends a <see cref="string" /> to the specified <see cref="InternedString" />.
        /// </summary>
        /// <param name="prefix">The <see cref="InternedString" /> prefix.</param>
        /// <param name="suffix">The <see cref="string" /> suffix.</param>
        /// <returns>
        ///     An <see cref="InternedString" /> representing the concatenation of <paramref name="prefix" /> and
        ///     <paramref name="suffix" />.
        /// </returns>
        public static InternedString operator +(InternedString prefix, string suffix)
        {
            return prefix == null ? new InternedString(suffix) : new InternedString(prefix.Node[suffix]);
        }
    }
}