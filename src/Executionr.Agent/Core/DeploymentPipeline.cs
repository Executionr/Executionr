using System;
using Raven.Client;
using NLog;
using Executionr.Agent.Domain;
using Executionr.Agent.Core.Steps;
using System.Collections.Generic;

namespace Executionr.Agent.Core
{
    public class DeploymentPipeline : IDeploymentPipeline
    {
        private static Logger Log = LogManager.GetCurrentClassLogger();

        private IDocumentSession _session;
        private IEnumerable<IDeploymentStep> _steps;

        public DeploymentPipeline(IDocumentSession session, IEnumerable<IDeploymentStep> steps)
        {
            _session = session;
            _steps = steps;
        }

        public void Deploy(Deployment deployment)
        {
            Log.Info("Beginning deployment...");

            deployment.State = DeploymentState.Running;
            _session.Store(deployment);
            _session.SaveChanges();

            try
            {
                foreach (var step in _steps)
                {
                    Log.Info("Running {0}...", step.GetType().Name);
                    step.Run(deployment, new {});
                }

                deployment.State = DeploymentState.Completed;
                _session.Store(deployment);
                _session.SaveChanges();
            }
            catch (Exception ex)
            {
                deployment.State = DeploymentState.Failed;
                _session.Store(deployment);
                _session.SaveChanges();
            }
        }

        #region IDisposable implementation

        public void Dispose()
        {
        }

        #endregion
    }
}

