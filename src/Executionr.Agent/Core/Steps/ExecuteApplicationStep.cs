using System;
using Executionr.Agent.Domain;
using Executionr.Agent.IO;
using System.IO;
using System.Diagnostics;
using Raven.Client;

namespace Executionr.Agent.Core.Steps
{
    public class ExecuteApplicationStep : IDeploymentStep
    {
        private const string FileName = "execute";
        private readonly string[] Extensions;
        private IEnvironment _environment;
        private readonly IDocumentSession documentSession;

        public ExecuteApplicationStep(IEnvironment environment, IDocumentSession documentSession)
        {
            if (environment.IsUnix)
            {
                Extensions = new string[] { ".sh", ".exe" };
            } 
            else
            {
                Extensions = new string[] { ".bat", ".ps1", ".exe" };
            }

            _environment = environment;
            this.documentSession = documentSession;
        }

        
        public void Run(Execution execution, IDeploymentLogger log, dynamic state)
        {
            bool found = false;

            foreach (var extension in Extensions)
            {
                log.Info("Looking for {0}{1}...", FileName, extension);

                string filePath = Path.Combine(execution.ApplicationPath(), string.Concat(FileName, extension));

                if (File.Exists(filePath))
                {
                    log.Info("Executing {0}{1}...", FileName, extension);

                    found = true;

                    if (_environment.IsUnix)
                    {
                        SetUnixExecPermission(filePath);
                    }

                    ExecuteScript(filePath, log, execution);
                    break;
                }
            }

            if (!found)
            {
                throw new FileNotFoundException("Could not find a Execution script in the application root.");
            }
        }

        
        private void SetUnixExecPermission(string filePath)
        {
            var attribs = File.GetAttributes(filePath);
            File.SetAttributes (filePath, (FileAttributes)((uint)attribs | 0x80000000)); 
        }

        private void ExecuteScript(string filePath, IDeploymentLogger log, Execution execution)
        {
            var startInfo = new ProcessStartInfo();
            startInfo.FileName = filePath;
            startInfo.WorkingDirectory = Path.GetDirectoryName(filePath);
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.CreateNoWindow = true;

            var process = Process.Start(startInfo);
            var outputStream = process.StandardOutput;
            var errorStream = process.StandardError;

            while (!process.WaitForExit(1000))
            {
                FlushStreamToLog(outputStream, log.Info, execution);
                FlushStreamToLog(errorStream, log.Error, execution);
            }

            FlushStreamToLog(outputStream, log.Info, execution);
            FlushStreamToLog(errorStream, log.Error, execution);

            if (process.ExitCode != 0)
            {
                throw new DeploymentException("The script exited with a non-zero exit code.");
            }
        }

        private void FlushStreamToLog(StreamReader stream, Action<string, object[]> log, Execution execution)
        {
            string line;
            while ((line = stream.ReadLine()) != null)
            {
                log(line, null);

                execution.Log.Add(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss: ") + line);
                documentSession.SaveChanges();
            }
        }
    }
}

