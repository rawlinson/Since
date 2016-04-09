using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Since.Rdf.Serialization
{
    public class JsonSerializer : IGraphSerializer
    {
        Newtonsoft.Json.JsonSerializer serializer = Newtonsoft.Json.JsonSerializer.CreateDefault();

        public async Task<IGraph> Deserialize(Stream stream, CancellationToken ct = default(CancellationToken))
        {
            JObject jobject;

            using (var reader = new StreamReader(stream))
            using (var j = new JsonTextReader(reader))
            {
                jobject = await Task.Factory.StartNew(() => serializer.Deserialize<JObject>(j), ct);
            }

            if (ct.IsCancellationRequested)
                return null;

            return this.Deserialize(jobject);
        }

        public IGraph Deserialize(JObject jobject)
        {
            throw new NotImplementedException();
        }

        public async Task Serialize(IGraph graph, Stream stream, CancellationToken ct = default(CancellationToken))
        {
            throw new NotImplementedException();
        }
    }
}
