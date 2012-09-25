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
        private IDocumentSession _session;
        private IEnumerable<IDeploymentStep> _steps;

        public DeploymentPipeline(IDocumentSession session, IEnumerable<IDeploymentStep> steps)
        {
            _session = session;
            _steps = steps;
        }

        public void Deploy(Deployment deployment)
        {
            var log = new DeploymentLogger(deployment, _session, this.GetType());
            log.Info("Beginning deployment...");

            deployment.State = DeploymentState.Running;
            _session.SaveChanges();

            try
            {
                var state = new {};
                foreach (var step in _steps)
                {
                    log.Info("Running {0}...", step.GetType().Name);
                    step.Run(deployment, new DeploymentLogger(deployment, _session, step.GetType()), state);
                }

                deployment.State = DeploymentState.Completed;
                _session.SaveChanges();
            }
            catch (Exception ex)
            {
                deployment.State = DeploymentState.Failed;
                _session.SaveChanges();

                log.ErrorException("Deployment failed.", ex);
            }
        }

        #region IDisposable implementation

        public void Dispose()
        {
        }

        #endregion
    }
}

