using AkademiaCsharp.Workers.Interfaces;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace AkademiaCsharp.Workers
{
    public class AsyncHasher : IAsyncHasher
    {
        private readonly string _hashAlgorithmName;

        public AsyncHasher(string hashAlgorithmName)
        {
            _hashAlgorithmName = hashAlgorithmName;
        }

        public async Task<byte[]> HashAsync(Stream stream, CancellationToken token)
        {
            var buffer = new byte[64 * 1024];
            using var hasher = HashAlgorithm.Create(_hashAlgorithmName);

            int bytesRead;
            while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, token)) != 0)
            {
                hasher.TransformBlock(buffer, 0, bytesRead, buffer, 0);
            }
            hasher.TransformFinalBlock(buffer, 0, bytesRead);

            return hasher.Hash;
        }
    }
}
