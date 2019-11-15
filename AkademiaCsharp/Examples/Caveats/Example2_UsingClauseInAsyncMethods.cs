using AkademiaCsharp.Workers;
using AkademiaCsharp.Workers.Interfaces;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AkademiaCsharp.Examples.Caveats
{
    public class Example2_UsingClauseInAsyncMethods : IExample
    {
        private readonly IAsyncHasher _asyncHasher;
        private readonly int _streamLength;
        private readonly int _seed;

        public Example2_UsingClauseInAsyncMethods(IAsyncHasher asyncHasher, int streamLength, int seed)
        {
            _asyncHasher = asyncHasher;
            _streamLength = streamLength;
            _seed = seed;
        }

        public async Task<bool> InvokeAsync(ITimeMeasurer timeMeasurer, CancellationToken token)
        {
            await ComputeHash(token);
            return true;
        }

        private Task<byte[]> ComputeHash( CancellationToken token)
        {
            using (var stream = new RandomStream(_streamLength, _seed))
            {
                return _asyncHasher.HashAsync(stream, token);
            }
        }
    }
}
