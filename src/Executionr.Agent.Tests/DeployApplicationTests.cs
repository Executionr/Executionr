using System;
using NUnit.Framework;
using Executionr.Agent.Core.Steps;
using Executionr.Agent.Domain;
using Executionr.Agent.IO;
using Rhino.Mocks;
using Executionr.Agent.Core;
using System.IO;

namespace Executionr.Agent.Tests
{
    [TestFixture]
    public class DeployApplicationTests
    {
        [Test]
        public void StepExecutesDeploymentScript()
        {
            var deployment = new Deployment()
            {
                Id = 1,
                State = DeploymentState.Scheduled,
                Url = new Uri("http://www.executionr.net/packages/1.nupkg"),
                Version = "1.0.0.0"
            };
            var logger = MockRepository.GenerateStub<IDeploymentLogger>();
            var environment = new Executionr.Agent.Core.Environment();

            RunDeployment(deployment, environment, logger, new {});

            // TODO: Actually assert the script was called (by leaving a trace file from the script)
            logger.AssertWasCalled(l => l.Info("Executing {0}{1}...", "deploy", ".sh"));
        }

        private void RunDeployment(Deployment deployment, IEnvironment environment, IDeploymentLogger logger, dynamic state)
        {
            var step = new DeployApplicationStep(environment);
            var applicationPath = deployment.ApplicationPath();

            // Copy deployment script to application dir
            if (!Directory.Exists(applicationPath))
            {
                Directory.CreateDirectory(applicationPath);
            }
            
            File.Copy(Path.Combine("Assets", "deploy.sh"), Path.Combine(applicationPath, "deploy.sh"));
            
            try
            {
                step.Run(deployment, logger, new {});
                logger.AssertWasCalled(l => l.Info("Executing {0}{1}...", "deploy", ".sh"));
            }
            finally
            {           
                if (Directory.Exists(applicationPath))
                {
                    Directory.Delete(applicationPath, true);
                }
            }
        }
    }
}

