using System;
using Executionr.Agent.Domain;
using System.IO;
using Executionr.Agent.IO;

namespace Executionr.Agent.Core.Steps
{
    public class UnpackPackageStep : IDeploymentStep
    {
        #region IDeploymentStep implementation

        public void Run(Deployment deployment, dynamic state)
        {
            string packagePath = deployment.PackagePath();
            string applicationPath = deployment.ApplicationPath();


        }

        #endregion
    }
}

