using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Since.Text;

namespace Since.Net
{
    public class NamespaceMapper
    {
        private readonly Dictionary<InternedStringNode, string> _nodeToPrefix =
            new Dictionary<InternedStringNode, string>();

        private readonly Dictionary<string, InternedStringNode> _prefixToNode =
            new Dictionary<string, InternedStringNode>();

        public Iri this[string prefix]
        {
            get
            {
                Contract.Requires(prefix != null);

                InternedStringNode node;
                return _prefixToNode.TryGetValue(prefix, out node) ? new Iri(node) : null;
            }
            set
            {
                Contract.Requires(prefix != null);
                Contract.Requires(value != null);

                _prefixToNode[prefix] = value.Node;
                _nodeToPrefix[value.Node] = prefix;
            }
        }

        public IReadOnlyDictionary<string, InternedStringNode> Items
            => _prefixToNode;

        public Iri Resolve(string qname)
        {
            Contract.Requires(qname != null);

            var i = qname.IndexOf(':');
            if (i < 0)
                return new Iri(qname);

            InternedStringNode node;
            return _prefixToNode.TryGetValue(qname.Substring(0, i), out node)
                ? new Iri(node[qname.Substring(i + 1)])
                : new Iri(qname);
        }

        public string Compact(Iri iri)
        {
            Contract.Requires(iri != null);

            var node = iri.Node;
            string prefix = null;
            var localName = "";

            while (node != null)
            {
                if (_nodeToPrefix.TryGetValue(node, out prefix))
                    break;
                localName = node.Value + localName;
                node = node.Parent;
            }

            return prefix == null ? localName : $"{prefix}:{localName}";
        }

        public override string ToString()
            => string.Join(" ", this.Items.Select(e => $"xmlns:{e.Key}={e.Value.ToAbsoluteString()}"));
    }
}