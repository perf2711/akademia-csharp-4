using AkademiaCsharp.Workers.Interfaces;
using System.IO;
using System.Security.Cryptography;

namespace AkademiaCsharp.Workers
{
    public class SyncHasher : ISyncHasher
    {
        private readonly string _hashAlgorithmName;

        public SyncHasher(string hashAlgorithmName)
        {
            _hashAlgorithmName = hashAlgorithmName;
        }

        public byte[] Hash(Stream stream)
        {
            using var hasher = HashAlgorithm.Create(_hashAlgorithmName);
            return hasher.ComputeHash(stream);
        }
    }
}
