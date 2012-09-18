using System;
using Nancy;
using System.Collections.Generic;
using Raven.Client;
using Raven.Client.Linq;
using Nancy.ModelBinding;
using Nancy.Responses;
using System.Linq;
using Executionr.Agent.Core;
using Executionr.Agent.Extensions;
using Nancy.Validation;

namespace Executionr.Agent
{
	public class DeployModule : NancyModule
	{
        private IDocumentSession _session;
        private IObjectMapper _mapper;

        public DeployModule(IDocumentSession session, IObjectMapper mapper) : base("/deployments")
		{
            this._session = session;
            this._mapper = mapper;

            Get["/{id}"] = OnGet;
            Get["/"] = OnList;
            Post["/"] = OnAdd;
		}

        private dynamic OnGet(dynamic arg)
        {
            int id = arg.id;
            var deployment = _session.Query<Domain.Deployment>().SingleOrDefault(d => d.Id == id);

            if (deployment != null)
            {
                return Response.AsJson(_mapper.Map<Domain.Deployment, Model.Deployment>(deployment));
            } 
            else
            {
                return 404;
            }
        }

		private dynamic OnList(dynamic arg)
		{
            return Response.AsJson(_mapper.Map<Domain.Deployment, Model.Deployment>(_session.Query<Domain.Deployment>()));
		}

        private dynamic OnAdd(dynamic arg)
        {
            var model = this.Bind<Model.Deployment>();
            var result = this.Validate(model);
            var deployment = _mapper.Map<Model.Deployment, Domain.Deployment>(model);

            if (result.IsValid)
            {
                _session.Store(deployment);
                _session.SaveChanges();

                return Response.AsCreated("/deployments/" + deployment.Id);
            } 
            else
            {
                return Response.AsValidationError(result);
            }
		}
	}
}

