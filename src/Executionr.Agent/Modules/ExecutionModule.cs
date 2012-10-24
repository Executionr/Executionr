using System;
using System.ComponentModel.DataAnnotations;
using Executionr.Agent.Domain;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Validation;
using Raven.Client;
using System.Linq;

namespace Executionr.Agent.Modules
{
    public class ExecutionModule : NancyModule
    {
        public ExecutionModule(IDocumentSession documentSession)
        {
            Post["/execution/"] = parameters =>
            {
                var model = this.Bind<CreateExecutionInputModel>();
                var validationResult = this.Validate(model);
                
                if(!validationResult.IsValid)
                {
                    var errorResponse = Response.AsJson(validationResult.Errors);
                    errorResponse.StatusCode = HttpStatusCode.BadRequest;
                    return errorResponse;
                }

                var executionToCreate = new Execution();
                executionToCreate.CreateDate = DateTime.Now;
                executionToCreate.LastUpdated = DateTime.Now;
                executionToCreate.UrlToPackage = new Uri(model.UrlToPackage);
                executionToCreate.State = ExecutionState.Scheduled;
                executionToCreate.ExecutionArguments = model.ExecutionArguments;
                
                documentSession.Store(executionToCreate);
                documentSession.SaveChanges();

                return Response.AsJson(new {ExecutionId = executionToCreate.Id});
            };

            Get["/execution/{ExecutionId}"] = parameters =>
                {
                    var result = documentSession.Load<Execution>((int) parameters.ExecutionId);
                    return Response.AsJson(result);
                };

            Get["/executions/"] = parameters =>
            {
                var result = documentSession.Query<Execution>().OrderByDescending(e => e.CreateDate).Take(1000).ToList();
                return Response.AsJson(result);
            };
        }

    }

    public class CreateExecutionInputModel
    {
        [Required]
        public string UrlToPackage { get; set; }
        public string ExecutionArguments { get; set; }
    }
}

