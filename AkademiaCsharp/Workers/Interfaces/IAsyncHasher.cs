using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AkademiaCsharp.Workers.Interfaces
{
    public interface IAsyncHasher
    {
        Task<byte[]> HashAsync(Stream stream, CancellationToken token);
    }
}
