using System;

namespace Executionr.Agent.Domain
{
    public class Deployment
    {
        public int Id { get; private set; }
        public string Version { get; set; }
        public Uri Url { get; set; }
        public string Hash { get; set; }
        public DeploymentState State { get; set; }
        public DateTime CreateDate { get; set; }
    }
}

