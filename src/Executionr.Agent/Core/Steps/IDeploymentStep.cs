using System;
using Executionr.Agent.Domain;

namespace Executionr.Agent.Core.Steps
{
    public interface IDeploymentStep
    {
        void Run(Deployment deployment, dynamic state);
    }
}

