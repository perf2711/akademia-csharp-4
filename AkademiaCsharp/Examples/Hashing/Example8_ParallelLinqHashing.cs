using AkademiaCsharp.Extensions;
using AkademiaCsharp.Workers;
using AkademiaCsharp.Workers.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace AkademiaCsharp.Examples.Hashing
{
    public class Example8_ParallelLinqHashing : IExample
    {
        private readonly ISyncHasher _hasher;
        private readonly int _streamLength;
        private readonly int _seed;
        private readonly int _count;

        public Example8_ParallelLinqHashing(ISyncHasher hasher, int streamLength, int seed, int count)
        {
            _hasher = hasher;
            _streamLength = streamLength;
            _seed = seed;
            _count = count;
        }

        public Task<bool> InvokeAsync(ITimeMeasurer timeMeasurer, CancellationToken token)
        {
            var hashes = new List<byte[]>(_count);
            var lockObject = new object();

            timeMeasurer.Start();
            Parallel.For(0, _count, (_) =>
            {
                using var randomStream = new RandomStream(_streamLength, _seed);

                var result = _hasher.Hash(randomStream);
                lock (lockObject)
                {
                    hashes.Add(result);
                }
            });
            timeMeasurer.Stop();

            foreach (var hash in hashes)
            {
                Console.WriteLine($"Hash: {hash.ToString("x2")}");
            }

            return Task.FromResult(hashes.AreAllEqual());
        }
    }
}
