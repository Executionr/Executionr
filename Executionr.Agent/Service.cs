using System;
using Nancy.Hosting.Self;
using Topshelf;

namespace Executionr.Agent
{
    public class Service : ServiceControl
    {
        private NancyHost _host;

        public Service()
        {
            _host = new NancyHost(new Uri("http://localhost:12345"));
        }

        #region ServiceControl implementation

        public bool Start(HostControl hostControl)
        {
            _host.Start();

            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            _host.Stop();

            return true;
        }

        #endregion
    }
}

