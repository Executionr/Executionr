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
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private readonly string[] Extensions;

        public DeployApplicationStep(IEnvironment environment)
        {
            if (environment.IsNix)
            {
                Extensions = new string[] { ".sh", ".exe" };
            } 
            else
            {
                Extensions = new string[] { ".bat", ".ps1", ".exe" };
            }
        }

        #region IDeploymentStep implementation

        public void Run(Deployment deployment, dynamic state)
        {
            string dir = deployment.ApplicationPath();
            bool found = false;

            foreach (var extension in Extensions)
            {
                Log.Info("Looking for {0}{1}...", FileName, extension);

                string filePath = Path.Combine(dir, string.Concat(FileName, extension));

                if (File.Exists(filePath))
                {
                    found = true;
                    ExecuteScript(filePath, dir);
                    break;
                }
            }

            if (!found)
            {
                throw new FileNotFoundException("Could not find a deployment script in the application root.");
            }
        }

        #endregion

        private void ExecuteScript(string filePath, string workingDirectory)
        {
            var startInfo = new ProcessStartInfo(filePath);
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.WorkingDirectory = workingDirectory;

            var process = Process.Start(startInfo);
            var outputStream = process.StandardOutput;
            var errorStream = process.StandardError;

            while (!process.WaitForExit(1000))
            {
                FlushStreamToLog(outputStream, Log.Info);
                FlushStreamToLog(errorStream, Log.Error);
            }

            FlushStreamToLog(outputStream, Log.Info);
            FlushStreamToLog(errorStream, Log.Error);

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

