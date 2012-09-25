using System;
using Executionr.Agent.Domain;
using NLog;
using System.IO;
using Executionr.Agent.IO;
using Executionr.Agent.Net;

namespace Executionr.Agent.Core.Steps
{
    public class DownloadPackageStep : IDeploymentStep
    {
        private IHasher _hasher;
        private IWebClient _webClient;

        public DownloadPackageStep(IWebClient webClient, IHasher hasher)
        {
            _webClient = webClient;
            _hasher = hasher;
        }

        #region IDeploymentStep implementation

        public void Run(Deployment deployment, IDeploymentLogger log, dynamic state)
        {
            string path = deployment.PackagePath();
            bool download = true;

            log.Info("Checking package cache...", deployment.Url);

            if (File.Exists(path))
            {
                log.Info("Package has already been downloaded, checking hash...");

                using (var stream = File.OpenRead(path))
                {
                    if (_hasher.ValidateHash(stream, deployment.Hash))
                    {
                        log.Info("Hashes match, no need to download the package again.");
                        download = false;
                    }
                    else
                    {
                        log.Info("Hashes do NOT match, the package must be downloaded again.");
                        File.Delete(path);
                    }
                }
            }

            if (download)
            {
                log.Info("Dowloading package @ {0}...", deployment.Url);

                _webClient.DownloadFile(deployment.Url, path);
            }

            using (var stream = File.OpenRead(path))
            {
                if (!_hasher.ValidateHash(stream, deployment.Hash))
                {
                    throw new DeploymentException("The hashes did not match after downloading the package.");
                }
            }
        }

        #endregion
    }
}

