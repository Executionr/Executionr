using System;

namespace Executionr.Agent.Core
{
    public interface IDeploymentLogger : IDisposable
    {
        void Info(string message, params object[] args);
        void Error(string message, params object[] args);
        void ErrorException(string message, Exception ex);
    }
}

