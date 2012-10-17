using System;
using Raven.Client;
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

        public void Deploy(Execution execution)
        {
            var log = new DeploymentLogger(execution, _session, this.GetType());
            log.Info("Beginning Execution...");

            execution.State = ExecutionState.Running;
            _session.SaveChanges();

            try
            {
                var state = new {};
                foreach (var step in _steps)
                {
                    log.Info("Running {0}...", step.GetType().Name);
                    step.Run(execution, new DeploymentLogger(execution, _session, step.GetType()), state);
                }

                execution.State = ExecutionState.Completed;
                _session.SaveChanges();
            }
            catch (Exception ex)
            {
                execution.State = ExecutionState.Failed;
                _session.Store(execution);
                _session.SaveChanges();

                log.ErrorException("Execution failed.", ex);
            }
        }

        #region IDisposable implementation

        public void Dispose()
        {
        }

        #endregion
    }
}

