using System;
using Executionr.Agent.Domain;
using System.Net;
using NLog;
using System.IO;
using Executionr.Agent.IO;

namespace Executionr.Agent.Core.Steps
{
    public class DownloadPackageStep : IDeploymentStep
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private IHasher _hasher;

        public DownloadPackageStep(IHasher hasher)
        {
            _hasher = hasher;
        }

        #region IDeploymentStep implementation

        public void Run(Deployment deployment, dynamic state)
        {
            string path = deployment.PackagePath();
            bool download = true;

            Log.Info("Checking package cache...", deployment.Url);

            if (File.Exists(path))
            {
                Log.Warn("Package has already been downloaded, checking hash...");

                using (var stream = File.OpenRead(path))
                {
                    if (_hasher.ValidateHash(stream, deployment.Hash))
                    {
                        Log.Info("Hashes match, no need to download the package again.");
                        download = false;
                    }
                    else
                    {
                        Log.Info("Hashes do NOT match, the package must be downloaded again.");
                        File.Delete(path);
                    }
                }
            }

            if (download)
            {
                Log.Info("Dowloading package @ {0}...", deployment.Url);

                WebClient client = new WebClient();
                client.DownloadFile(deployment.Url, path);
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

