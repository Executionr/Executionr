using System.ComponentModel;
using System.ServiceProcess;

namespace Executionr.Agent
{
    [RunInstaller(true)]
    public class ExecutionrServiceProcessInstaller : ServiceProcessInstaller
    {
        public ExecutionrServiceProcessInstaller()
        {
            this.Account = ServiceAccount.LocalSystem;
        }
    }

    [RunInstaller(true)]
    public class ExecutionrServiceInstaller : ServiceInstaller
    {
        public ExecutionrServiceInstaller()
        {
            this.Description = "The package executionr";
            this.DisplayName = "Executionr Agent";
            this.ServiceName = "ExecutionrAgent";
            this.StartType = ServiceStartMode.Automatic;
        }
    }
}
