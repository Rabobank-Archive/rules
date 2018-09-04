using System;
using Xunit;
using Shouldly;
using System.Net;
using Microsoft.Extensions.Configuration;
using RestSharp;
using lib.Vsts;

namespace lib.tests
{
    public class VstsRestClientTest : IClassFixture<TestConfig>
    {
        private readonly TestConfig config;
        private readonly IVstsRestClient vsts;

        public VstsRestClientTest(TestConfig config)
        {
            this.config = config;
            vsts = new VstsRestClient(config.Organization, config.Token);
        }

        [Fact]
        public void QueryReleaseDefinitions()
        {
            var definitions = vsts.Execute(Requests.Release.Definitions(config.Project));

            definitions.StatusCode.ShouldBe(HttpStatusCode.OK);
            definitions.Data.Value.ShouldAllBe(_ => !string.IsNullOrEmpty(_.Name));
        }

        [Fact]
        public void QueryReleaseDefinitionDetails()
        {
            var definition = vsts.Execute(Requests.Release.Definition(config.Project, "2"));

            definition.StatusCode.ShouldBe(HttpStatusCode.OK);
            definition.Data.Name.ShouldBe("demo SOx");
        }

        [Fact]
        public void QueryServiceConnections()
        {
            var definition = vsts.Execute(Requests.ServiceEndpoint.Endpoints(config.Project));

            definition.StatusCode.ShouldBe(HttpStatusCode.OK);
            definition.Data.Value.ShouldNotBeEmpty();
            definition.Data.Value.ShouldAllBe(e => !string.IsNullOrEmpty(e.Name));
            definition.Data.Value.ShouldAllBe(e => !string.IsNullOrEmpty(e.Id));
        }

        [Fact]
        public void QueryServiceConnectionHistory()
        {
            var history = vsts.Execute(Requests.ServiceEndpoint.History(config.Project, "975b3603-9939-4f22-a5a9-baebb39b5dad"));
            history.StatusCode.ShouldBe(HttpStatusCode.OK);
            history.Data.Value.ShouldNotBeEmpty();
            history.Data.Value.ShouldAllBe(e => e.Data != null);
            history.Data.Value.ShouldAllBe(e => e.Data.Definition != null);
            history.Data.Value.ShouldAllBe(e => !string.IsNullOrEmpty(e.Data.Definition.Id));
        }
    }
}
