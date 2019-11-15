using System.IO;

namespace AkademiaCsharp.Workers.Interfaces
{
    public interface ISyncHasher
    {
        byte[] Hash(Stream stream);
    }
}
