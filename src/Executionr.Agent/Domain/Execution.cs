using System;
using System.Collections.Generic;

namespace Executionr.Agent.Domain
{
    public class Execution
    {
        public Execution()
        {
            Log = new List<string>();
        }

        public int Id { get; set; }
        public ExecutionState State { get; set; }
        public IList<string> Log { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastUpdated { get; set; }

        public string PackageId { get; set; }
        public string PackageVersion { get; set; }
        public Uri UrlToPackage { get; set; }
        public string ExecutionArguments { get; set; }
    }
}

