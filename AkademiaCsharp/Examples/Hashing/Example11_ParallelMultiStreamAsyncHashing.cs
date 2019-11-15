using AkademiaCsharp.Extensions;
using AkademiaCsharp.Workers;
using AkademiaCsharp.Workers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace AkademiaCsharp.Examples.Hashing
{
    public class Example11_ParallelMultiStreamAsyncHashing : IExample
    {
        private readonly IAsyncHasher _hasher;
        private readonly int _streamLength;
        private readonly int _seed;
        private readonly int _count;

        public Example11_ParallelMultiStreamAsyncHashing(IAsyncHasher hasher, int streamLength, int seed, int count)
        {
            _hasher = hasher;
            _streamLength = streamLength;
            _seed = seed;
            _count = count;
        }

        public async Task<bool> InvokeAsync(ITimeMeasurer timeMeasurer, CancellationToken token)
        {
            var hashTasks = new List<Task<byte[]>>(_count);

            timeMeasurer.Start();
            for (var i = 0; i < _count; i++)
            {
                hashTasks.Add(Hash(_hasher, token));
            }
            await Task.WhenAll(hashTasks);
            timeMeasurer.Stop();

            foreach (var hashTask in hashTasks)
            {
                Console.WriteLine($"Hash: {hashTask.Result.ToString("x2")}");
            }

            return hashTasks.Select(t => t.Result).AreAllEqual();
        }

        private async Task<byte[]> Hash(IAsyncHasher hasher, CancellationToken token)
        {
            using var randomStream = new RandomStream(_streamLength, _seed);
            return await hasher.HashAsync(randomStream, token);
        }
    }
}
