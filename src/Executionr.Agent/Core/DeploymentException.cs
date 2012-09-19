using System;

namespace Executionr.Agent.Core
{
    [Serializable]
    public class DeploymentException : Exception
    {
        public DeploymentException(string message) : base(message)
        {
        }
    }
}

