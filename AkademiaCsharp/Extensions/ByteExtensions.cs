using System.Collections.Generic;
using System.Linq;

namespace AkademiaCsharp.Extensions
{
    public static class ByteExtensions
    {
        public static string ToString(this IEnumerable<byte> bytes, string format)
        {
            return string.Join("", bytes.Select(b => b.ToString(format)));
        }

        public static bool AreAllEqual(this IEnumerable<byte[]> byteArrays)
        {
            byte[] firstArray = null;

            foreach (var byteArray in byteArrays)
            {
                if (firstArray == null)
                {
                    firstArray = byteArray;
                    continue;
                }

                if (firstArray.Length != byteArray.Length)
                {
                    return false;
                }

                for (var i=0; i<firstArray.Length; i++)
                {
                    if (firstArray[i] != byteArray[i])
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
