using System;
using Nancy.Hosting.Self;
using NLog.Config;
using NLog.Targets;
using NLog;

namespace Executionr.Agent
{
	class MainClass
	{
		public static void Main (string[] args)
		{
            ConfigureLogging();
            InitializeHost();
		}

        static void ConfigureLogging()
        {
            LoggingConfiguration config = new LoggingConfiguration();

            ColoredConsoleTarget consoleTarget = new ColoredConsoleTarget();
            config.AddTarget("console", consoleTarget);
            consoleTarget.Layout = "${date:format=HH\\:MM\\:ss} ${logger} ${message}";

            LoggingRule rule = new LoggingRule("*", LogLevel.Debug, consoleTarget);
            config.LoggingRules.Add(rule);

            LogManager.Configuration = config;
        }

        static void InitializeHost()
        {
            var host = new NancyHost(new Uri("http://localhost:12345"));

            host.Start();

            Console.ReadLine();

            host.Stop();
        }
	}
}
