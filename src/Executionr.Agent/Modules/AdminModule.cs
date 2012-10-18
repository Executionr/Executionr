using System.Linq;
using Executionr.Agent.Domain;
using Nancy;
using Raven.Client;

namespace Executionr.Agent.Modules
{
    public class AdminModule : NancyModule
    {
        public AdminModule(IDocumentSession documentSession)
        {
            Get["/admin"] = parameters =>
            {
                return View["static/admin.html"];
            };

            Get["/admin/executions"] = parameters =>
            {
                var result = documentSession.Query<Execution>().OrderByDescending(e => e.CreateDate).Take(1000).ToList().Select(e => new
                    {
                        e.Id, 
                        e.PackageId, 
                        e.PackageVersion, 
                        CreateDate = e.CreateDate.ToString("yyyy-MM-dd hh:mm:ss"),
                        Status = GetStatus(e.State),
                        Style = GetStyle(e.State),
                    });
                return Response.AsJson(result);
            };
        }

        public string GetStatus(ExecutionState state)
        {
            if (state == ExecutionState.Failed)
                return "Failed";

            if (state == ExecutionState.Cancelled)
                return "Cancelled";

            if (state == ExecutionState.Completed)
                return "Completed";

            if (state == ExecutionState.Running)
                return "Running";

            return "Scheduled";
        }

        public string GetStyle(ExecutionState state)
        {
            if (state == ExecutionState.Failed)
                return "label-important";

            if (state == ExecutionState.Cancelled)
                return "label-important";

            if (state == ExecutionState.Completed)
                return "label-success";

            if (state == ExecutionState.Running)
                return "label-info";

            return "label-inverse";
        }
    }
}
