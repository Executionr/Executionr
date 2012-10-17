using System;
using System.Configuration;
using Nancy.Hosting.Self;
using NLog.Config;
using NLog.Targets;
using NLog;

namespace Executionr.Agent
{
    public class MainClass
    {
        public static void Main(string[] args)
        {
            ConfigureLogging();
            InitializeHost();
        }

        static void ConfigureLogging()
        {
            LoggingConfiguration config = new LoggingConfiguration();

            MemoryTarget target = new MemoryTarget();
            target.Layout = "${message}";

            SimpleConfigurator.ConfigureForTargetLogging(target, LogLevel.Debug);

            ColoredConsoleTarget consoleTarget = new ColoredConsoleTarget();
            config.AddTarget("console", consoleTarget);
            consoleTarget.Layout = "${date:format=HH\\:MM\\:ss} ${logger} ${message}";

            LoggingRule rule = new LoggingRule("*", LogLevel.Debug, consoleTarget);
            config.LoggingRules.Add(rule);

            LogManager.Configuration = config;
        }

        static void InitializeHost()
        {
            var hostUri = ConfigurationManager.AppSettings["HostUri"];
            if (string.IsNullOrEmpty(hostUri))
                hostUri = "http://localhost:12345";
            var host = new NancyHost(new Uri(hostUri));

            host.Start();

            Console.ReadLine();

            host.Stop();
        }
    }
}
