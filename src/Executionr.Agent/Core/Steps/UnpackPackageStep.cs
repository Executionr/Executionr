using System;
using Executionr.Agent.Domain;
using System.IO;
using Executionr.Agent.IO;
using ICSharpCode.SharpZipLib;
using NLog;
using ICSharpCode.SharpZipLib.Zip;

namespace Executionr.Agent.Core.Steps
{
    public class UnpackPackageStep : IDeploymentStep
    {
        #region IDeploymentStep implementation

        public void Run(Deployment deployment, IDeploymentLogger log, dynamic state)
        {
            string packagePath = deployment.PackagePath();
            string applicationPath = deployment.ApplicationPath();

            log.Info("Checking zip file integrity...");

            using (var zipFile = new ZipFile(File.OpenRead(packagePath)))
            {
                log.Info("Unpacking zip file...");

                applicationPath.EnsureDirectoryExists();
                zipFile.IsStreamOwner = true;

                foreach (ZipEntry entry in zipFile)
                {
                    string path = Path.Combine(applicationPath, entry.Name);

                    if (entry.IsDirectory && !Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    else if(entry.IsFile)
                    {
                        string dir = Path.GetDirectoryName(path);
                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }

                        using (var output = File.Create(path))
                        {
                            zipFile.GetInputStream(entry).CopyTo(output);
                        }
                    }
                }

                zipFile.Close();
            }
        }

        #endregion
    }
}

