using System;
using System.IO;
using System.Xml.Linq;
using Executionr.Agent.Domain;
using Executionr.Agent.IO;
using Raven.Client;
using System.Linq;

namespace Executionr.Agent.Core.Steps
{
    public class ReadManifestStep : IDeploymentStep
    {
        private readonly IDocumentSession documentSession;

        public ReadManifestStep(IDocumentSession documentSession)
        {
            this.documentSession = documentSession;
        }

        public void Run(Execution execution, IDeploymentLogger logger, dynamic state)
        {
            execution.Log.Add(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss: ") + "Looking for manifest (nuspec-file).");
            documentSession.SaveChanges();

            var unpackedPath = execution.UnpackingPath();
            var unpackedDirectory = new DirectoryInfo(unpackedPath);

            var file = unpackedDirectory.GetFiles("*.nuspec").FirstOrDefault();

            execution.Log.Add(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss: ") + "Manifest (nuspec-file) " + file.FullName + " found.");
            documentSession.SaveChanges();


            var nuspec = XDocument.Load(file.FullName);
            var id = nuspec.Descendants().FirstOrDefault(x => x.Name.LocalName == "id").Value;
            var version = nuspec.Descendants().FirstOrDefault(x => x.Name.LocalName == "version").Value;

            execution.PackageId = id;
            execution.PackageVersion = version;

            execution.Log.Add(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss: ") + "Package id: " + id + ", Package version: " + version);
            execution.Log.Add(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss: ") + "Moving content to application directory " + execution.ApplicationPath());
            documentSession.SaveChanges();

            var applicationPath = execution.ApplicationPath();

            if (Directory.Exists(applicationPath))
                Directory.Delete(applicationPath, true);

            Directory.Move(unpackedPath, applicationPath);

            execution.Log.Add(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss: ") + "Moved content to application directory " + execution.ApplicationPath());
            documentSession.SaveChanges();
        }
    }
}
