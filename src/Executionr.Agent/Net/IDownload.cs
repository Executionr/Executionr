using System;
using System.IO;

namespace Executionr.Agent
{
    public interface IDownload : IDisposable
    {
        Uri Url { get; }
        Stream Stream { get; }
    }
}

