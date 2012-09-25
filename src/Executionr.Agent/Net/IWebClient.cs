using System;

namespace Executionr.Agent.Net
{
    public interface IWebClient
    {
        void DownloadFile(Uri address, string fileName);
    }
}

