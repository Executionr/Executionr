using System;
using System.Threading;
using Raven.Client;
using System.Linq;
using Executionr.Agent.Domain;
using Executionr.Agent.Core.Steps;
using System.Collections.Generic;

namespace Executionr.Agent.Core
{
    public class DeploymentWatcher : IDeploymentWatcher
    {
        private Thread _thread;
        private bool _keepRunning;
        private Func<IDocumentSession> _documentSessionFactory;
        private Func<IDeploymentPipeline> _pipelineFactory;

        public DeploymentWatcher(Func<IDocumentSession> documentSessionFactory, Func<IDeploymentPipeline> pipelineFactory)
        {
            _documentSessionFactory = documentSessionFactory;
            _pipelineFactory = pipelineFactory;
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

                using (var session = _documentSessionFactory()) 
                {
                    var deployments = session.Query<Execution>()
                                                .Where(d => d.State == ExecutionState.Scheduled)
                                                .OrderBy(d => d.CreateDate)
                                                .Take(5);

                    foreach (var deployment in deployments) 
                    {
                        using (var pipeline = _pipelineFactory()) 
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

