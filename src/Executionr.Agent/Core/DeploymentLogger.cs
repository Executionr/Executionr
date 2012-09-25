using System;
using NLog;
using Executionr.Agent.Domain;
using Raven.Client;

namespace Executionr.Agent.Core
{
    public class DeploymentLogger : IDeploymentLogger
    {
        private Logger _logger;
        private Deployment _deployment;
        private IDocumentSession _session;

        public DeploymentLogger(Deployment deployment, IDocumentSession session, Type loggerType)
        {
            _deployment = deployment;
            _session = session;
            _logger = LogManager.GetLogger("DeploymentLogger", loggerType);
        }

        #region IDeploymentLogger implementation

        public void Info(string message, params object[] args)
        {
            _logger.Info(message, args);

            if (args != null && args.Length > 0)
            {
                _deployment.Log.Add(string.Format(message, args));
            }
            else
            {
                _deployment.Log.Add(message);
            }

            if (_deployment.Log.Count % 5 == 0)
            {
                Flush();
            }
        }

        public void Error(string message, params object[] args)
        {
            _logger.Error(message, args);
            
            if (args != null && args.Length > 0)
            {
                _deployment.Log.Add(string.Format(message, args));
            }
            else
            {
                _deployment.Log.Add(message);
            }
            
            if (_deployment.Log.Count % 5 == 0)
            {
                Flush();
            }
        }

        public void ErrorException(string message, Exception ex)
        {
            _logger.ErrorException(message, ex);
            _deployment.Log.Add(message + ex);

            if (_deployment.Log.Count % 5 == 0)
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

