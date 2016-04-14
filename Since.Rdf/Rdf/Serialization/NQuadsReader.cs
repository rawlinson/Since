using Since.Net;
using Since.Rdf.Serialization.Grammar.NQuads;
using System;
using System.Collections.Generic;
using System.IO;

namespace Since.Rdf.Serialization
{
    public class NQuadsReader
    {
        private TextReader _reader;

        public NQuadsReader(TextReader reader)
        {
            _reader = reader;
        }

        bool TrySplit(string line, char delimiter, ref string head, ref string tail)
        {
            var index = line.IndexOf(delimiter);
            if (index == -1)
                return false;
            head = line.Substring(0, index);
            tail = line.Substring(index + 1);
            return true;
        }

        bool TrySplitAny(string line, char[] delimiters, ref string head, ref string tail)
        {
            var index = line.IndexOfAny(delimiters);
            if (index == -1)
                return false;
            head = line.Substring(0, index);
            tail = line.Substring(index + 1);
            return true;
        }

        bool TryReadIri(string line, ref string iri, ref string tail)
        {
            if (!line.StartsWith("<"))
                return false;
            return TrySplit(line.Substring(1), '>', ref iri, ref tail);
        }

        bool TryReadBlankNode(string line, ref string name, ref string tail)
        {
            if (!line.StartsWith("_:"))
                return false;
            return TrySplitAny(line.Substring(2), new char[] { ' ', '\t', '.' }, ref name, ref tail);
        }

        bool TryReadLiteral(string line, ref string value, ref Iri datatype, ref string languageCode, ref string tail)
        {
            if (!line.StartsWith("\""))
                return false;

            int index = line.LastIndexOf('\"');
            if (index < 1)
                return false;

            value = line.Substring(1, index - 1);
            tail = line.Substring(index + 1);

            return true;
        }

        Dictionary<string, BlankNode> blankNodes = new Dictionary<string, BlankNode>();

        BlankNode GetOrCreateBlankNode(string id)
        {
            BlankNode node;
            if (!blankNodes.TryGetValue(id, out node))
            {
                node = new BlankNode();
                blankNodes.Add(id, node);
            }
            return node;
        }

        bool TryReadQuad(string line, out Edge edge)
        {
            edge = null;

            line = line.Trim();
            if (String.IsNullOrEmpty(line) || line.StartsWith("#"))
                return false;

            string temp = "";

            INode subject;
            if (TryReadIri(line, ref temp, ref line))
            {
                subject = new IriNode(new Iri(temp));
            }
            else if (TryReadBlankNode(line, ref temp, ref line))
            {
                subject = GetOrCreateBlankNode(temp);
            }
            else
                return false;

            line = line.TrimStart();

            INode predicate;
            if (TryReadIri(line, ref temp, ref line))
            {
                predicate = new IriNode(new Iri(temp));
            }
            else
                return false;

            line = line.TrimStart();

            INode obj;
            if (TryReadIri(line, ref temp, ref line))
            {
                obj = new IriNode(new Iri(temp));
            }
            else if (TryReadBlankNode(line, ref temp, ref line))
            {
                obj = GetOrCreateBlankNode(temp);
            }
            else if (line.StartsWith("\""))
            {
                string value = "", languageCode = "";
                Iri dataType = null;
                if (!TryReadLiteral(line, ref value, ref dataType, ref languageCode, ref line))
                    return false;
                if (dataType == null)
                    obj = new LiteralNode(value, languageCode);
                else
                    obj = new LiteralNode(value, dataType);
            }
            else
                return false;

            line = line.TrimStart();

            INode context = null;
            if (TryReadIri(line, ref temp, ref line))
            {
                context = new IriNode(new Iri(temp));
            }
            else if (TryReadBlankNode(line, ref temp, ref line))
            {
                context = GetOrCreateBlankNode(temp);
            }

            edge = new Edge(subject, predicate, obj, context);

            return true;
        }

        public Edge ReadQuad()
        {
            Edge quad = null;
            TryReadQuad(_reader.ReadLine(), out quad);
            return quad;
        }

        public IEnumerable<Edge> Triples()
        {
            var stream = new Antlr4.Runtime.AntlrInputStream(_reader);
            var lexer = new NQuadsLexer(stream);
            var parser = new NQuadsParser(new Antlr4.Runtime.CommonTokenStream(lexer));
            
            foreach (var child in parser.nquadsDoc().statement())
            {
                yield return null;
            }
        }

        public IEnumerable<Edge> Quads()
        {
            string line = _reader.ReadLine();
            while (line != null)
            {
                Edge quad;
                if (TryReadQuad(line, out quad))
                    yield return quad;
                line = _reader.ReadLine();
            }
        }
    }



}
