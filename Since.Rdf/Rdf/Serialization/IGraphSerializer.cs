using System.Diagnostics.Contracts;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Since.Rdf.Serialization
{
    [ContractClass(typeof(IGraphSerializerContracts))]
    public interface IGraphSerializer
    {/*
        Task Serialize(IGraph graph, Stream stream, CancellationToken ct = new CancellationToken());
        Task<IGraph> Deserialize(Stream stream, CancellationToken ct = new CancellationToken());
        */
    }

    [ContractClassFor(typeof(IGraphSerializer))]
    internal abstract class IGraphSerializerContracts : IGraphSerializer
    {
        /*
        async Task IGraphSerializer.Serialize(IGraph graph, Stream stream, CancellationToken ct)
        {
            Contract.Requires(graph != null);
            Contract.Requires(stream != null);
            Contract.Requires(stream.CanWrite);
            Contract.Requires(ct != null);
        }

        async Task<IGraph> IGraphSerializer.Deserialize(Stream stream, CancellationToken ct)
        {
            Contract.Requires(stream != null);
            Contract.Requires(stream.CanWrite);
            Contract.Requires(ct != null);

            return default(IGraph);
        }
        */
    }
}