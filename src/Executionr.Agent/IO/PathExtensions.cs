using System;
using Executionr.Agent.Domain;
using System.IO;

namespace Executionr.Agent.IO
{
    public static class PathExtensions
    {
        public static string PackagePath(this Deployment deployment)
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Packages", string.Format("{0}.pkg", deployment.Id));
        }

        public static string ApplicationPath(this Deployment deployment)
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Applications", deployment.Id.ToString());
        }
    }
}

