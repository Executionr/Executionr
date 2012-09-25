using System;
using Executionr.Agent.Domain;
using Executionr.Agent.IO;
using NLog;
using System.IO;
using System.Diagnostics;

namespace Executionr.Agent.Core.Steps
{
    public class DeployApplicationStep : IDeploymentStep
    {
        private const string FileName = "deploy";
        private readonly string[] Extensions;
        private IEnvironment _environment;

        public DeployApplicationStep(IEnvironment environment)
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
        }

        #region IDeploymentStep implementation

        public void Run(Deployment deployment, IDeploymentLogger log, dynamic state)
        {
            bool found = false;

            foreach (var extension in Extensions)
            {
                log.Info("Looking for {0}{1}...", FileName, extension);

                string filePath = Path.Combine(deployment.ApplicationPath(), string.Concat(FileName, extension));

                if (File.Exists(filePath))
                {
                    log.Info("Executing {0}{1}...", FileName, extension);

                    found = true;

                    if (_environment.IsUnix)
                    {
                        SetUnixExecPermission(filePath);
                    }

                    ExecuteScript(filePath, log);
                    break;
                }
            }

            if (!found)
            {
                throw new FileNotFoundException("Could not find a deployment script in the application root.");
            }
        }

        #endregion

        private void SetUnixExecPermission(string filePath)
        {
            var attribs = File.GetAttributes(filePath);
            File.SetAttributes (filePath, (FileAttributes)((uint)attribs | 0x80000000)); 
        }

        private void ExecuteScript(string filePath, IDeploymentLogger log)
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
                FlushStreamToLog(outputStream, log.Info);
                FlushStreamToLog(errorStream, log.Error);
            }

            FlushStreamToLog(outputStream, log.Info);
            FlushStreamToLog(errorStream, log.Error);

            if (process.ExitCode != 0)
            {
                throw new DeploymentException("The script exited with a non-zero exit code.");
            }
        }

        private void FlushStreamToLog(StreamReader stream, Action<string, object[]> log)
        {
            string line;
            while ((line = stream.ReadLine()) != null)
            {
                log(line, null);
            }
        }
    }
}

