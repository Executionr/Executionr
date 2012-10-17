using System;
using Nancy;
using Raven.Client;
using Raven.Client.Embedded;
using Executionr.Agent.IO;
using Executionr.Agent.Core.Steps;
using Executionr.Agent.Net;

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
            container.Register((c, o) => c.Resolve<IDocumentStore>().OpenSession());
            container.Register<IObjectMapper, ObjectMapper>().AsSingleton();
            container.Register<IHasher, Sha256Hasher>().AsSingleton();
            container.Register<IEnvironment, Environment>().AsSingleton();
            container.RegisterMultiple<IDeploymentStep>(new [] { typeof(DownloadPackageStep), typeof(UnpackPackageStep), typeof(ReadManifestStep), typeof(ExecuteApplicationStep) }).AsMultiInstance();
            container.Register<IDeploymentPipeline, DeploymentPipeline>().AsMultiInstance();
            container.Register<IDeploymentWatcher, DeploymentWatcher>();
            container.Register<IWebClient, WebClient>();

            container.Resolve<IDeploymentWatcher>().Start();
        }
    }
}

