using System;

namespace Executionr.Agent.Core
{
    public interface IEnvironment
    {
        bool IsMono { get; }
        bool IsUnix { get; }
    }
}

