using AkademiaCsharp.Extensions;
using AkademiaCsharp.Workers;
using AkademiaCsharp.Workers.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AkademiaCsharp.Examples.Hashing
{
    public delegate byte[] HashMethodCaller();

    public class Example7_ApmHashing : IExample
    {
        private readonly IApmHasher _hasher;
        private readonly int _streamLength;
        private readonly int _seed;
        private readonly int _count;

        public Example7_ApmHashing(IApmHasher hasher, int streamLength, int seed, int count)
        {
            _hasher = hasher;
            _streamLength = streamLength;
            _seed = seed;
            _count = count;
        }

        public Task<bool> InvokeAsync(ITimeMeasurer timeMeasurer, CancellationToken token)
        {
            var hashes = new List<byte[]>(_count);
            var hashResults = new List<IAsyncResult>();

            timeMeasurer.Start();
            for (var i = 0; i < _count; i++)
            {
                var randomStream = new RandomStream(_streamLength, _seed);
                hashResults.Add(_hasher.BeginHash(randomStream, (state) =>
                {
                    (state.AsyncState as RandomStream).Dispose();
                }, randomStream));
            }

            foreach (var result in hashResults)
            {
                hashes.Add(_hasher.EndHash(result));
            }

            timeMeasurer.Stop();

            foreach (var hash in hashes)
            {
                Console.WriteLine($"Hash: {hash.ToString("x2")}");
            }

            return Task.FromResult(hashes.AreAllEqual());
        }
    }
}
