using System;

namespace Executionr.Agent.Core
{
    public interface IDeploymentWatcher : IDisposable
    {
        void Start();
        void Stop();
    }
}

