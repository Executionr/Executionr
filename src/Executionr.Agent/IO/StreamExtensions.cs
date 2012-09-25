using System;
using System.IO;

namespace Executionr.Agent.IO
{
    public static class StreamExtensions
    {
        public static void CopyTo(this Stream from, Stream to)
        {
            byte[] buff = new byte[4096];
            int read;
            while ((read = from.Read(buff, 0, buff.Length)) > 0)
            {
                to.Write(buff, 0, read);
            }
        }
    }
}

