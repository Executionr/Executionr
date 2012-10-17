using System;
using System.Configuration;
using Executionr.Agent.Domain;
using System.IO;

namespace Executionr.Agent.IO
{
    public static class PathExtensions
    {
        public static string PackagePath(this Execution execution)
        {
            var packagesDirectory = ConfigurationManager.AppSettings["PackagesDirectory"];
            if(string.IsNullOrEmpty(packagesDirectory))
                packagesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Packages");

            EnsureDirectoryExists(packagesDirectory);

            return Path.Combine(packagesDirectory, string.Format("{0}.nupkg", execution.Id));
        }

        public static string UnpackingPath(this Execution execution)
        {
            var unpackedPackagesDirectory = ConfigurationManager.AppSettings["UpackedPackagesDirectory"];
            if (string.IsNullOrEmpty(unpackedPackagesDirectory))
                unpackedPackagesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UpackedPackages");

            EnsureDirectoryExists(unpackedPackagesDirectory);

            return Path.Combine(unpackedPackagesDirectory, execution.Id.ToString());
        }

        public static string ApplicationPath(this Execution execution)
        {
            var versionName = string.Format("{0}.{1}", execution.PackageId, execution.PackageVersion);

            var applicationsDirectory = ConfigurationManager.AppSettings["ApplicationsDirectory"];

            if (string.IsNullOrEmpty(applicationsDirectory))
                applicationsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Applications");

            EnsureDirectoryExists(applicationsDirectory);

            return Path.Combine(applicationsDirectory, versionName);
        }

        public static void EnsureDirectoryExists(this string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }

    
}

