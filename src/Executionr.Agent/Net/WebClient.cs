using System;

namespace Executionr.Agent.Net
{
    public class WebClient : IWebClient
    {
        #region IWebClient implementation
        public void DownloadFile(Uri address, string fileName)
        {
            new System.Net.WebClient().DownloadFile(address, fileName);
        }
        #endregion
    }
}

