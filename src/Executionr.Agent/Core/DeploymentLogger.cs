using System;
using NLog;
using Executionr.Agent.Domain;
using Raven.Client;

namespace Executionr.Agent.Core
{
    public class DeploymentLogger : IDeploymentLogger
    {
        private Logger _logger;
        private Execution execution;
        private IDocumentSession _session;

        public DeploymentLogger(Execution execution, IDocumentSession session, Type loggerType)
        {
            this.execution = execution;
            _session = session;
            _logger = LogManager.GetLogger("DeploymentLogger");
        }

        #region IDeploymentLogger implementation

        public void Info(string message, params object[] args)
        {
            _logger.Info(message, args);

            if (args != null && args.Length > 0)
            {
                execution.Log.Add(string.Format(message, args));
            }
            else
            {
                execution.Log.Add(message);
            }

            if (execution.Log.Count % 5 == 0)
            {
                Flush();
            }
        }

        public void Error(string message, params object[] args)
        {
            _logger.Error(message, args);
            
            if (args != null && args.Length > 0)
            {
                execution.Log.Add(string.Format(message, args));
            }
            else
            {
                execution.Log.Add(message);
            }
            
            if (execution.Log.Count % 5 == 0)
            {
                Flush();
            }
        }

        public void ErrorException(string message, Exception ex)
        {
            _logger.ErrorException(message, ex);
            execution.Log.Add(message + ex);

            if (execution.Log.Count % 5 == 0)
            {
                Flush();
            }
        }

        #endregion

        #region IDisposable implementation

        public void Dispose()
        {
            Flush();
        }

        #endregion

        private void Flush()
        {
            _session.SaveChanges();
        }
    }
}

