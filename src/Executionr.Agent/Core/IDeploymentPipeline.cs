using System;
using Executionr.Agent.Domain;

namespace Executionr.Agent.Core
{
    public interface IDeploymentPipeline : IDisposable
    {
        void Deploy(Deployment deployment);
    }
}

