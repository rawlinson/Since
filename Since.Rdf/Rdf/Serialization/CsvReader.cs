//using AngleSharp.Parser.Html;
using Since.Net;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Since.Rdf.Serialization
{
    public class CsvReader
    {
        TextReader reader;
        char delimiter = ',';
        char qualifier = '"';

        public CsvReader(TextReader reader)
        {
            this.reader = reader;
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
                    {
                        Subject = blank,
                        Predicate = new IriNode(new Iri(gen + headers[i])),
                        Object = new LiteralNode(record[i].Trim()),
                        Context = context
                    };
            }
        }

        public IEnumerable<IList<string>> Records()
        {
            var record = new List<string>();
            var field = new StringBuilder();

            bool quote = false;

            while (reader.Peek() != -1)
            {
                var c = (char)reader.Read();

                if (quote)
                {
                    if (c == qualifier)
                    {
                        if (reader.Peek() == qualifier)
                            field.Append((char)reader.Read());
                        else
                            quote = false;
                    }
                    else
                        field.Append(c);
                }
                else
                {
                    if (c == delimiter)
                    {
                        record.Add(field.ToString());
                        field.Clear();
                    }
                    else if (c == qualifier)
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
