using System;
using Xunit;
using Shouldly;
using System.Net;
using Microsoft.Extensions.Configuration;
using lib.tests.clients;
using lib.tests.response;

namespace lib.tests
{
    public class UnitTest1
    {
        private readonly string token;

        public UnitTest1()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            token = configuration["token"];
        }

        [Fact]
        public void Test1()
        {
            string organization = "somecompany";
            var client = new VsrmClient(organization, token);

            var definitions = client.Execute<JsonCollection<ReleaseDefinition>>(Requests.ReleaseDefinitions("SOx-compliant-demo"));

            definitions.StatusCode.ShouldBe(HttpStatusCode.OK);
            definitions.Data.Value.ShouldAllBe(_ => !string.IsNullOrEmpty(_.Name));
        }

        [Fact]
        public void ReleaseDefinitionDetails()
        {
            string organization = "somecompany";
            var client = new VsrmClient(organization, token);

            var request = Requests.ReleaseDefinition("SOx-compliant-demo", "2");
            var definition = client.Execute<ReleaseDefinition>(request);

            definition.StatusCode.ShouldBe(HttpStatusCode.OK);
            definition.Data.Name.ShouldBe("demo SOx");
        }

        [Fact]
        public void QueryServiceConnections()
        {
            string organization = "somecompany";
            var client = new VstsClient(organization, token);

            var definition = client.Execute<JsonCollection<ServiceEndpoint>>(Requests.ServiceEndpoints("SOx-compliant-demo"));

            definition.StatusCode.ShouldBe(HttpStatusCode.OK);
            definition.Data.Value.ShouldContain(e => e.Name == "p02-prd-devautsox-deploy (SPN)");
        }
    }
}
