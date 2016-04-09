using Since.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Since.Rdf.Serialization
{
    public class SigSerializer : IGraphSerializer
    {
        /*
        static readonly string magic = "\u0089SIG\r\n\u001A\n";
        static readonly byte[] magicBytes = Encoding.UTF8.GetBytes(magic);

        int version = 1;

        public string ContentType
            => "application/rdf+since";

        public async Task<IGraph> Deserialize(Stream stream, CancellationToken ct = new CancellationToken())
        {
            Contract.Requires(stream != null);
            Contract.Requires(stream.CanRead);
            Contract.Requires(ct != null);

            var graph = new Graph();

            var buffer = new byte[magicBytes.Length];
            stream.Read(buffer, 0, buffer.Length);
            if (!buffer.SequenceEqual(magicBytes))
                return (IGraph)graph;           

            return (IGraph)graph;
        }

        public async Task Serialize(IGraph graph, Stream stream, CancellationToken ct = new CancellationToken())
        {
            Contract.Requires(graph != null);
            Contract.Requires(stream != null);
            Contract.Requires(stream.CanWrite);
            Contract.Requires(ct != null);

            using (var writer = new BinaryWriter(stream, Encoding.UTF8))
            {
                this.WriteHeader(graph, writer, ct);
                this.WriteNodes(graph, stream, ct);
                this.WriteTriples(graph, writer, ct);
            }
        }

        private void WriteHeader(IGraph graph, BinaryWriter writer, CancellationToken ct)
        {
            writer.Write(magic);
            writer.Write(version);
            writer.Write(graph.NodeCount);
            writer.Write(graph.TripleCount);
        }

        private Dictionary<INode, int> _nodeMap = null;

        private void WriteNodes(IGraph graph, Stream stream, CancellationToken ct)
        {
            _nodeMap = new Dictionary<INode, int>(graph.NodeCount);

            int index = 0;
            foreach (var node in graph.Nodes)
            {
                this.WriteNode(node, stream, ct);
                _nodeMap[node] = index++;
            }
        }

        private void WriteNode(INode node, Stream stream, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        private void WriteTriples(IGraph graph, BinaryWriter writer, CancellationToken ct)
        {
            Contract.Requires(_nodeMap != null);

            foreach (var triple in graph.Triples)
            {
                writer.WriteVarInt(_nodeMap[triple.Subject]);
                writer.WriteVarInt(_nodeMap[triple.Predicate]);
                writer.WriteVarInt(_nodeMap[triple.Object]);
                writer.WriteVarInt(_nodeMap[triple.Context]);
            }
        }

        #region NullTerminatedString

        List<byte> readNullTerminatedStringBytes = new List<byte>();
        string ReadNullTerminatedString(TextReader reader)
        {
            readNullTerminatedStringBytes.Clear();

            int c;
            while ((c = reader.Read()) > 0)
                readNullTerminatedStringBytes.Add((byte)c);

            return Encoding.UTF8.GetString(readNullTerminatedStringBytes.ToArray());
        }

        void WriteNullTerminatedString(string str, BinaryWriter writer)
        {
            writer.Write(str);
            writer.Write((byte)0);
        }

        #endregion
        
        #region InternedString

        private Dictionary<InternedStringNode, int> _mpa = new Dictionary<InternedStringNode, int>();

        private void WriteInternedString(InternedString str, TextWriter writer, CancellationToken ct = new CancellationToken())
        {
            int id;
            if (_mpa.TryGetValue(str.Node, out id))
                writer.Write(id);
            else
                throw new InvalidOperationException($"InternedString {nameof(str)} does not appear in the dictionary");
        }

        private void WriteInternedStringNode(InternedStringNode node, BinaryWriter writer, int index, CancellationToken ct = new CancellationToken())
        {
            this.WriteNullTerminatedString(node.Value, writer);
            _mpa[node] = index;
            foreach (var n in node.Children)
                this.WriteInternedStringNode(n, writer, ++index, ct);
            this.WriteNullTerminatedString(string.Empty, writer);
        }

        private InternedStringNode ReadInternedStringNode(TextReader reader, CancellationToken ct = new CancellationToken())
        {
            var root = new InternedStringNode {Value = this.ReadNullTerminatedString(reader)};
            
            var node = root;
            while (true)
            {
                var value = this.ReadNullTerminatedString(reader);
                if (string.IsNullOrEmpty(value))
                {
                    node = node.Parent;
                    if (node == null)
                        break;
                }
                else
                {
                    var next = new InternedStringNode
                    {
                        Value = value,
                        Parent = root
                    };
                    node.Children.Add(next);
                    node = next;
                }
            }

            return root;
        }

        #endregion
        */
    }
}
