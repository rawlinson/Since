//using AngleSharp.Parser.Html;
using Since.Net;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Since.Rdf.Serialization
{
    public class CsvReader
    {
        private static readonly char DELIMITER_CHAR = ',';
        private static readonly char QUALIFIER_CHAR = '"';

        TextReader _reader;

        public CsvReader(TextReader reader)
        {
            _reader = reader;
        }

        public IEnumerable<Edge> Quads()
        {
            string gen = @"http://genestry.com/2015/terms#";
            BlankNode context = new BlankNode();

            string[] headers = new string[] { "surname", "firstname", "birth", "location", "url1", "url2" };
            
            foreach (var record in this.Records())
            {
                BlankNode blank = new BlankNode();
                for (int i = 0; i < 6; i++)
                    yield return new Edge
                    (
                        subject: blank,
                        predicate: new IriNode(new Iri(gen + headers[i])),
                        obj: new LiteralNode(record[i].Trim()),
                        context: context
                    );
            }
        }

        public IEnumerable<IList<string>> Records()
        {
            var record = new List<string>();
            var field = new StringBuilder();

            bool quote = false;

            while (_reader.Peek() != -1)
            {
                var c = (char)_reader.Read();

                if (quote)
                {
                    if (c == QUALIFIER_CHAR)
                    {
                        if (_reader.Peek() == QUALIFIER_CHAR)
                            field.Append((char)_reader.Read());
                        else
                            quote = false;
                    }
                    else
                        field.Append(c);
                }
                else
                {
                    if (c == DELIMITER_CHAR)
                    {
                        record.Add(field.ToString());
                        field.Clear();
                    }
                    else if (c == QUALIFIER_CHAR)
                    {
                        quote = true;
                    }
                    else if (c == '\r' || c == '\n')
                    {
                        if (record.Count > 0 || field.Length > 0)
                        {
                            record.Add(field.ToString());
                            field.Clear();
                        }
                        if (record.Count > 0)
                        {
                            yield return record;
                            record = new List<string>(record.Count);
                        }
                    }
                    else
                    {
                        field.Append(c);
                    }
                }
            }

            if (field.Length > 0)
                record.Add(field.ToString());
            if (record.Count > 0)
                yield return record;
        }
    }


  
}
