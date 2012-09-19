using System;

namespace Executionr.Agent.Domain
{
    public enum DeploymentState
    {
        Scheduled = 0,
        Running = 1,
        Cancelled = 2,
        Failed = 3,
        Completed = 4
    }
}

