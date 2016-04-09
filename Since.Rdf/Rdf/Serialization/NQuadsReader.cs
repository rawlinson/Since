using Since.Net;
using Since.Rdf.Serialization.Grammar.NQuads;
using System;
using System.Collections.Generic;
using System.IO;

namespace Since.Rdf.Serialization
{
    public class NQuadsReader
    {
        private TextReader reader;

        public NQuadsReader(TextReader reader)
        {
            this.reader = reader;
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

        bool TryReadQuad(string line, out Edge quad)
        {
            quad = new Edge();

            line = line.Trim();
            if (String.IsNullOrEmpty(line) || line.StartsWith("#"))
                return false;

            string temp = "";
            if (TryReadIri(line, ref temp, ref line))
            {
                quad.Subject = new IriNode(new Iri(temp));
            }
            else if (TryReadBlankNode(line, ref temp, ref line))
            {
                quad.Subject = GetOrCreateBlankNode(temp);
            }
            else
                return false;

            line = line.TrimStart();

            if (TryReadIri(line, ref temp, ref line))
            {
                quad.Predicate = new IriNode(new Iri(temp));
            }
            else
                return false;

            line = line.TrimStart();

            if (TryReadIri(line, ref temp, ref line))
            {
                quad.Object = new IriNode(new Iri(temp));
            }
            else if (TryReadBlankNode(line, ref temp, ref line))
            {
                quad.Object = GetOrCreateBlankNode(temp);
            }
            else if (line.StartsWith("\""))
            {
                string value = "", languageCode = "";
                Iri dataType = null;
                if (!TryReadLiteral(line, ref value, ref dataType, ref languageCode, ref line))
                    return false;
                if (dataType == null)
                    quad.Object = new LiteralNode(value, languageCode);
                else
                    quad.Object = new LiteralNode(value, dataType);
            }
            else
                return false;

            line = line.TrimStart();

            if (TryReadIri(line, ref temp, ref line))
            {
                quad.Context = new IriNode(new Iri(temp));
            }
            else if (TryReadBlankNode(line, ref temp, ref line))
            {
                quad.Context = GetOrCreateBlankNode(temp);
            }

            return true;
        }

        public Edge ReadQuad()
        {
            Edge quad = null;
            TryReadQuad(reader.ReadLine(), out quad);
            return quad;
        }

        public IEnumerable<Edge> Triples()
        {
            var stream = new Antlr4.Runtime.AntlrInputStream(reader);
            var lexer = new NQuadsLexer(stream);
            var parser = new NQuadsParser(new Antlr4.Runtime.CommonTokenStream(lexer));
            
            foreach (var child in parser.nquadsDoc().statement())
            {
                yield return new Edge();
            }
        }

        public IEnumerable<Edge> Quads()
        {
            string line = reader.ReadLine();
            while (line != null)
            {
                Edge quad;
                if (TryReadQuad(line, out quad))
                    yield return quad;
                line = reader.ReadLine();
            }
        }
    }



}
