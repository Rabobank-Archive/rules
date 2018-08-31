using System;
using RestSharp;
using Xunit;
using Shouldly;
using System.Net;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

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

            var definitions = client.Execute<JsonCollection<ReleaseDefinition>>(Requests.ReleaseDefinitions());

            definitions.StatusCode.ShouldBe(HttpStatusCode.OK);
            definitions.Data.Value.ShouldAllBe(_ => !string.IsNullOrEmpty(_.Name));
        }

        abstract class AuthorizedClient : RestClient
        {
            private readonly string token;

            public AuthorizedClient(string url, string token) : base(url)
            {
                this.token = token;
            }

            public override IRestResponse<T> Execute<T>(IRestRequest request)
            {
                return base.Execute<T>(request.AddHeader("authorization", GenerateAuthorizationHeader(token)));
            }
        }

        class VsrmClient : AuthorizedClient
        {
            public VsrmClient(string organization, string token) : base($"https://{organization}.vsrm.visualstudio.com/{{project}}/_apis/{{area}}/{{resource}}/", token)
            {
            }
        }


        class ReleaseDefinition 
        {
            public string Name { get; set; }
        }

        [Fact]
        public void ReleaseDefinitionDetails()
        {
            string organization = "somecompany";
            var client = new VsrmClient(organization, token);

            var request = Requests.ReleaseDefinition("2");
            var definition = client.Execute<ReleaseDefinition>(request);

            definition.StatusCode.ShouldBe(HttpStatusCode.OK);
            definition.Data.Name.ShouldBe("demo SOx");
        }

        static class Requests 
        {
            public static IRestRequest ReleaseDefinition(string id)
            {
                return new RestRequest(id, Method.GET)
                    .AddUrlSegment("project", "SOx-compliant-demo")
                    .AddUrlSegment("area", "release")
                    .AddUrlSegment("resource", "definitions");
            }

            public static IRestRequest ServiceEndpoints()
            {
                return new RestRequest(Method.GET)
                    .AddUrlSegment("project", "SOx-compliant-demo")
                    .AddUrlSegment("area", "serviceendpoint")
                    .AddUrlSegment("resource", "endpoints");
            }

            public static IRestRequest ReleaseDefinitions()
            {
                return new RestRequest(Method.GET)
                    .AddUrlSegment("project", "SOx-compliant-demo")
                    .AddUrlSegment("area", "release")
                    .AddUrlSegment("resource", "definitions");
            }
        }

        class ServiceEndpoint 
        {
            public string Name { get; set; }
        }

        class JsonCollection<T> {
            public int Count { get; set; }
            public List<T> Value { get; set; }
        }

        [Fact]
        public void QueryServiceConnections()
        {
            string organization = "somecompany";
            var client = new VstsClient(organization, token);

            var definition = client.Execute<JsonCollection<ServiceEndpoint>>(Requests.ServiceEndpoints());

            definition.StatusCode.ShouldBe(HttpStatusCode.OK);
            definition.Data.Value.ShouldContain(e => e.Name == "p02-prd-devautsox-deploy (SPN)");
        }

        class VstsClient : AuthorizedClient 
        {
            public VstsClient(string organization, string token) : base($"https://{organization}.visualstudio.com/{{project}}/_apis/{{area}}/{{resource}}/", token)
            {
            }
        }

        private static string GenerateAuthorizationHeader(string token)
        {
            string encoded = Base64Encode($":{token}");
            return ($"Basic {encoded}");
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}
