using System;
using System.Configuration;
using System.ServiceProcess;
using Nancy.Hosting.Self;

namespace Executionr.Agent
{
    partial class ExecutionrService : ServiceBase
    {
        public ExecutionrService()
        {
            InitializeComponent();
        }

        private static NancyHost host;

        protected override void OnStart(string[] args)
        {
            var hostUri = ConfigurationManager.AppSettings["HostUri"];
            if (string.IsNullOrEmpty(hostUri))
                hostUri = "http://localhost:12345";
            host = new NancyHost(new Uri(hostUri));

            host.Start();
        }

        protected override void OnStop()
        {
            if(host != null)
                host.Stop();
        }
    }
}
