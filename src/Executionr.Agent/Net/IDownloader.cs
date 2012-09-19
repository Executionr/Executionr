using System;

namespace Executionr.Agent
{
    public interface IDownloader
    {
        void Download<TState>(Uri uri, Action<TState, IDownload> progress, Action<TState, IDownload> completed);
    }
}

