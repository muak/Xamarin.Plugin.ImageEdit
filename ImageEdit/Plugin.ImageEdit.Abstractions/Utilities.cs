using System;
using System.Threading.Tasks;
using System.IO;

namespace Plugin.ImageEdit.Abstractions
{
    internal static class Utilities
    {
        internal static async Task<byte[]> StreamToBytes(Stream stream)
        {
            using (var ms = new MemoryStream()) {
                var buff = new byte[16 * 1024];
                int read;
                while ((read = await stream.ReadAsync(buff, 0, buff.Length)) > 0) {
                    await ms.WriteAsync(buff, 0, read);
                }

                stream.Dispose();

                return ms.ToArray();
            }
        }
    }
}
