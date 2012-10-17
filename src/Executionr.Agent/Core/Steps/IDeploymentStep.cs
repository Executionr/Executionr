using System;
using Executionr.Agent.Domain;

namespace Executionr.Agent.Core.Steps
{
    public interface IDeploymentStep
    {
        void Run(Execution execution, IDeploymentLogger logger, dynamic state);
    }
}

