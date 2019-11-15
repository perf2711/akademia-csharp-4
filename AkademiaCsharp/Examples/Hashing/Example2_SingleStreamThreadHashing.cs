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
    public class Example2_SingleStreamThreadHashing : IExample
    {
        private readonly ISyncHasher _hasher;
        private readonly int _streamLength;
        private readonly int _seed;
        private readonly int _count;

        public Example2_SingleStreamThreadHashing(ISyncHasher hasher, int streamLength, int seed, int count)
        {
            _hasher = hasher;
            _streamLength = streamLength;
            _seed = seed;
            _count = count;
        }

        public Task<bool> InvokeAsync(ITimeMeasurer timeMeasurer, CancellationToken token)
        {
            using var randomStream = new RandomStream(_streamLength, _seed);

            var hashes = new List<byte[]>(_count);
            var hashThreads = new List<Thread>(_count);

            for (var i = 0; i < _count; i++)
            {
                var thread = new Thread(() =>
                {
                    var result = _hasher.Hash(randomStream);
                    hashes.Add(result);
                });

                hashThreads.Add(thread);
            }

            timeMeasurer.Start();
            foreach (var thread in hashThreads)
            {
                thread.Start();
            }

            foreach (var thread in hashThreads)
            {
                thread.Join();
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
