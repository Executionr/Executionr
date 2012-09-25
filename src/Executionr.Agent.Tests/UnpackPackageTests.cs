using System;
using NUnit.Framework;
using Executionr.Agent.Core.Steps;
using Executionr.Agent.IO;
using Executionr.Agent.Domain;
using Rhino.Mocks;
using Executionr.Agent.Core;
using System.IO;
using System.Linq;

namespace Executionr.Agent.Tests
{
    [TestFixture]
    public class UnpackPackageTests
    {
        [Test]
        public void StepUnpacksPackage()
        {
            var step = new UnpackPackageStep();
            var deployment = new Deployment()
            {
                Id = 1,
                State = DeploymentState.Scheduled,
                Url = new Uri("http://www.executionr.net/packages/1.nupkg"),
                Version = "1.0.0.0"
            };
            var packagePath = deployment.PackagePath();
            var applicationPath = deployment.ApplicationPath();
            var logger = MockRepository.GenerateStub<IDeploymentLogger>();

            // Copy asset to package location
            var packageDir = new FileInfo(packagePath).Directory;

            if (!packageDir.Exists)
            {
                packageDir.Create();
            }

            File.Copy(Path.Combine("Assets", "RavenDB.Database.1.0.968.nupkg"), packagePath, true);

            try
            {
                // Unpack the package
                step.Run(deployment, logger, new {});
                
                // Verify it's been unpacked
                Assert.True(Directory.Exists(applicationPath));
                Assert.Contains("RavenDB.Database.nuspec", new DirectoryInfo(applicationPath).GetFiles().Select(f => f.Name).ToList());
            }
            finally
            {
                if (File.Exists(packagePath))
                {
                    File.Delete(packagePath);
                }
                
                if (Directory.Exists(applicationPath))
                {
                    Directory.Delete(applicationPath, true);
                }
            }
        }
    }
}