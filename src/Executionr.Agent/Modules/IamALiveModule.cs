using System.Linq;
using Executionr.Agent.Domain;
using Nancy;
using Raven.Client;

namespace Executionr.Agent.Modules
{
    public class IamALiveModule : NancyModule
    {
        public IamALiveModule(IDocumentSession documentSession)
        {
            Get["/"] = parameters =>
            {
                return Response.AsJson(new { NumberOfExecutions = documentSession.Query<Execution>().Count() });
            };
        }
    }
}
