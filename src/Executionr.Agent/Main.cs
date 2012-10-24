using System;
using System.Collections;
using System.Configuration;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
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
            if(args.Any(x => x.Equals("-install", StringComparison.InvariantCultureIgnoreCase)))
            {
                InstallService(args);
                return;
            }

            if (args.Any(x => x.Equals("-uninstall", StringComparison.InvariantCultureIgnoreCase)))
            {
                UninstallService(args);
                return;
            }

            if (args.Any(x => x.Equals("-console", StringComparison.InvariantCultureIgnoreCase)))
            {
                ConfigureLogging();
                InitializeHost();
                return;
            }

            ServiceBase[] services = { new ExecutionrService(),  };
            ServiceBase.Run(services);

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

        static void InstallService(string[] args)
        {
            try
            {
                Console.WriteLine("installing");
                using (AssemblyInstaller inst = new AssemblyInstaller(typeof(ExecutionrService).Assembly, args))
                {
                    IDictionary state = new Hashtable();
                    inst.UseNewContext = true;
                    try
                    {
                            inst.Install(state);
                            inst.Commit(state);
                    }
                    catch
                    {
                        try
                        {
                            inst.Rollback(state);
                        }
                        catch { }
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }

        static void UninstallService(string[] args)
        {
            try
            {
                Console.WriteLine("uninstalling");
                using (AssemblyInstaller inst = new AssemblyInstaller(typeof(MainClass).Assembly, args))
                {
                    IDictionary state = new Hashtable();
                    inst.UseNewContext = true;
                    try
                    {
                        inst.Uninstall(state);
                    }
                    catch
                    {
                        try
                        {
                            inst.Rollback(state);
                        }
                        catch { }
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }
    
    }
}
