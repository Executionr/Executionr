using System;
using System.Linq;
using Nancy;
using Nancy.Validation;
using System.Collections.Generic;

namespace Executionr.Agent.Extensions
{
    public static class ResponseExtensions
    {
        public static Response AsCreated(this IResponseFormatter response, string location)
        {
            return new Response()
                        .WithContentType("application/json")
                        .WithHeader("Location", location)
                        .WithStatusCode(201);
        }

        public static Response AsValidationError(this IResponseFormatter response, ModelValidationResult result)
        {
            var model = new { Errors = new List<string>() };

            foreach (var error in result.Errors)
            {
                foreach (var member in error.MemberNames) 
                {
                    model.Errors.Add(error.GetMessage(member));
                }
            }

            return response.AsJson(model)
                           .WithStatusCode(400);
        }
    }
}

