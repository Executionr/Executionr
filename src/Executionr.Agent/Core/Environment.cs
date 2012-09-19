using System;

namespace Executionr.Agent.Core
{
    public class Environment : IEnvironment
    {
        public bool IsMono
        {
            get { return Type.GetType("Mono.Runtime") != null; }
        }

        public bool IsNix
        {
            get { return System.Environment.OSVersion.Platform == PlatformID.Unix; }
        }
    }
}

