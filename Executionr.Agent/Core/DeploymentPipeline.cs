using System;
using Raven.Client;
using NLog;

namespace Executionr.Agent.Core
{
    public class DeploymentPipeline : IDisposable
    {
        private static Logger Log = LogManager.GetCurrentClassLogger();

        public DeploymentPipeline(IDocumentSession session)
        {
        }

        public void Deploy(Executionr.Agent.Domain.Deployment deployment)
        {
            Log.Info("Beginning deployment...");
        }

        #region IDisposable implementation

        public void Dispose()
        {
        }

        #endregion
    }
}

