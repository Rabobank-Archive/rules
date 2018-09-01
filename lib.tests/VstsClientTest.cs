using System;
using Xunit;
using Shouldly;
using System.Net;
using Microsoft.Extensions.Configuration;
using lib.Response;
using lib.Requests;
using RestSharp;

namespace lib.tests
{
    public class VstsClientTest
    {
        private const string project = "SOx-compliant-demo";
        private readonly IRestClient vsts = VstsClientFactory.Create();

        public VstsClientTest()
        {

        }

        [Fact]
        public void QueryReleaseDefinitions()
        {
            var definitions = vsts.Execute<JsonCollection<Response.ReleaseDefinition>>(new Requests.ReleaseDefinitions(project));

            definitions.StatusCode.ShouldBe(HttpStatusCode.OK);
            definitions.Data.Value.ShouldAllBe(_ => !string.IsNullOrEmpty(_.Name));
        }

        [Fact]
        public void QueryReleaseDefinitionDetails()
        {
            var definition = vsts.Execute<Response.ReleaseDefinition>(new Requests.ReleaseDefinition(project, "2"));

            definition.StatusCode.ShouldBe(HttpStatusCode.OK);
            definition.Data.Name.ShouldBe("demo SOx");
        }

        [Fact]
        public void QueryServiceConnections()
        {
            var definition = vsts.Execute<JsonCollection<ServiceEndpoint>>(new Requests.ServiceEndpoints(project));

            definition.StatusCode.ShouldBe(HttpStatusCode.OK);
            definition.Data.Value.ShouldNotBeEmpty();
            definition.Data.Value.ShouldAllBe(e => !string.IsNullOrEmpty(e.Name));
            definition.Data.Value.ShouldAllBe(e => !string.IsNullOrEmpty(e.Id));
        }

        [Fact]
        public void QueryServiceConnectionHistory()
        {
            var history = vsts.Execute<JsonCollection<Response.ServiceEndpointHistory>>(new Requests.ServiceEndpointHistory(project, "975b3603-9939-4f22-a5a9-baebb39b5dad"));
            history.StatusCode.ShouldBe(HttpStatusCode.OK);
            history.Data.Value.ShouldNotBeEmpty();
            history.Data.Value.ShouldAllBe(e => e.Data != null);
            history.Data.Value.ShouldAllBe(e => e.Data.Definition != null);
            history.Data.Value.ShouldAllBe(e => !string.IsNullOrEmpty(e.Data.Definition.Id));
        }
    }
}
