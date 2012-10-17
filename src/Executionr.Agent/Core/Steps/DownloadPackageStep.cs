using System;
using Executionr.Agent.Domain;
using System.IO;
using Executionr.Agent.IO;
using Executionr.Agent.Net;
using Raven.Client;

namespace Executionr.Agent.Core.Steps
{
    public class DownloadPackageStep : IDeploymentStep
    {
        private IWebClient _webClient;
        private readonly IDocumentSession documentSession;

        public DownloadPackageStep(IWebClient webClient, IDocumentSession documentSession)
        {
            _webClient = webClient;
            this.documentSession = documentSession;
        }

        #region IDeploymentStep implementation

        public void Run(Execution execution, IDeploymentLogger log, dynamic state)
        {
            string path = execution.PackagePath();
            
            log.Info("Checking package package exists...", execution.UrlToPackage);

            if (File.Exists(path))
            {
                log.Info("Package has already been downloaded. Deleting old package");
                File.Delete(path);
            }

            execution.Log.Add(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss: ") + "Starting download of package " + execution.UrlToPackage + " to destination " + path + ".");
            documentSession.SaveChanges();

            log.Info("Dowloading package @ {0}...", execution.UrlToPackage);
            _webClient.DownloadFile(execution.UrlToPackage, path);

            execution.Log.Add(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss: ") + "Finished download of package " + execution.UrlToPackage + " to destination " + path + ".");
            documentSession.SaveChanges();
            
        }

        #endregion
    }
}

