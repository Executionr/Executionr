using System;
using System.Collections.Generic;

namespace Executionr.Agent.Domain
{
    public class Deployment
    {
        public Deployment()
        {
            Log = new List<string>();
        }

        public int Id { get; set; }
        public string Version { get; set; }
        public Uri Url { get; set; }
        public string Hash { get; set; }
        public DeploymentState State { get; set; }
        public IList<string> Log { get; set; }
        public DateTime CreateDate { get; set; }
    }
}

