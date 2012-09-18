using System;
using System.Threading;
using Raven.Client;
using System.Linq;

namespace Executionr.Agent.Core
{
    public class DeploymentWatcher : IDisposable
    {
        private Thread _thread;
        private bool _keepRunning;
        private IDocumentStore _documentStore;

        public DeploymentWatcher(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
            _thread = new Thread(OnStart) 
            {
                IsBackground = true
            };
        }

        public void Start()
        {
            _keepRunning = true;
            _thread.Start();
        }

        public void Stop()
        {
            _keepRunning = false;

            if (_thread.IsAlive)
            {
                _thread.Join();
            }
        }

        private void OnStart(object state)
        {
            while (_keepRunning)
            {
                Thread.Sleep(TimeSpan.FromSeconds(20));

                using (var session = _documentStore.OpenSession()) 
                {
                    var deployments = session.Query<Domain.Deployment>().Take(5);

                    foreach (var deployment in deployments) 
                    {
                        using (var pipeline = new DeploymentPipeline(session)) 
                        {
                            pipeline.Deploy(deployment);
                        }
                    }
                }
            }
        }

        #region IDisposable implementation

        public void Dispose()
        {
            Stop();
        }

        #endregion
    }
}

