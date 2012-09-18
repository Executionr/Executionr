using System;
using Nancy;
using Raven.Client;
using Raven.Client.Embedded;

namespace Executionr.Agent.Core
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(TinyIoC.TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            var documentStore = new EmbeddableDocumentStore
            {
                DataDirectory = "Data"
            };
            documentStore.Configuration.DefaultStorageTypeName = "munin";
            documentStore.Initialize();

            container.Register<IDocumentStore>(documentStore);
            container.Register<IDocumentSession>((c, o) => c.Resolve<IDocumentStore>().OpenSession());
            container.Register<IObjectMapper, ObjectMapper>().AsSingleton();

            var deploymentWatcher = new DeploymentWatcher(documentStore);
            deploymentWatcher.Start();
            container.Register<DeploymentWatcher>(deploymentWatcher);
        }
    }
}

