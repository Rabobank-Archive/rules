using System;
using Xunit;
using Shouldly;
using System.Net;
using Microsoft.Extensions.Configuration;
using lib.tests.clients;
using lib.tests.Response;
using lib.tests.Requests;

namespace lib.tests
{
    public class UnitTest1
    {
        private readonly VsrmClient vsrm;
        private readonly VstsClient vsts;

        public UnitTest1()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var token = configuration["token"];

            string organization = "somecompany";
            vsrm = new VsrmClient(organization, token);
            vsts = new VstsClient(organization, token);
        }

        [Fact]
        public void QueryReleaseDefinitions()
        {
            var definitions = vsrm.Execute<JsonCollection<Response.ReleaseDefinition>>(new ReleaseDefinitions("SOx-compliant-demo"));

            definitions.StatusCode.ShouldBe(HttpStatusCode.OK);
            definitions.Data.Value.ShouldAllBe(_ => !string.IsNullOrEmpty(_.Name));
        }

        [Fact]
        public void QueryReleaseDefinitionDetails()
        {
            var definition = vsrm.Execute<Response.ReleaseDefinition>(new Requests.ReleaseDefinition("SOx-compliant-demo", "2"));

            definition.StatusCode.ShouldBe(HttpStatusCode.OK);
            definition.Data.Name.ShouldBe("demo SOx");
        }

        [Fact]
        public void QueryServiceConnections()
        {
            var definition = vsts.Execute<JsonCollection<ServiceEndpoint>>(new Requests.ServiceEndpoints("SOx-compliant-demo"));

            definition.StatusCode.ShouldBe(HttpStatusCode.OK);
            definition.Data.Value.ShouldContain(e => e.Name == "p02-prd-devautsox-deploy (SPN)");
        }
    }
}
