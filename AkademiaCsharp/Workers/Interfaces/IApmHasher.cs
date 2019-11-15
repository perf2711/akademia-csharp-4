using System;
using System.IO;

namespace AkademiaCsharp.Workers.Interfaces
{
    public interface IApmHasher
    {
        IAsyncResult BeginHash(Stream stream, AsyncCallback callback, object state);
        byte[] EndHash(IAsyncResult asyncResult);
    }
}
