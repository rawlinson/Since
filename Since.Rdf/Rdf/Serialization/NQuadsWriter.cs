using System.Collections.Generic;
using System.IO;

namespace Since.Rdf.Serialization
{
    public class NQuadsWriter
    {
        //http://www.w3.org/ns/formats/N-Assert
        TextWriter writer;

        public NQuadsWriter(TextWriter writer)
        {
            this.writer = writer;
        }

        public void Quads(IEnumerable<Edge> quads)
        {
            foreach (var quad in quads)
                writer.WriteLine(quad);
        }
    }
}
