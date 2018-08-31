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
            var client = new RestClient($"https://{organization}.vsrm.visualstudio.com/{{project}}/_apis/{{area}}/{{resource}}/");
            
            var request = new RestRequest(Method.GET);
            request
                .AddUrlSegment("project", "SOx-compliant-demo")
                .AddUrlSegment("area", "release")
                .AddUrlSegment("resource", "definitions");

            request.AddHeader("authorization", GenerateAuthorizationHeader(token));
            var definitions = client.Execute(request);
            
            definitions.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        class ReleaseDefinition 
        {
            public string Name { get; set; }
        }

        [Fact]
        public void ReleaseDefinitionDetails()
        {
            string organization = "somecompany";
            var client = new RestClient($"https://{organization}.vsrm.visualstudio.com/{{project}}/_apis/{{area}}/{{resource}}/");
            
            var request = new RestRequest("2", Method.GET);
            request
                .AddUrlSegment("project", "SOx-compliant-demo")
                .AddUrlSegment("area", "release")
                .AddUrlSegment("resource", "definitions");

            request.AddHeader("authorization", GenerateAuthorizationHeader(token));
            var definition = client.Execute<ReleaseDefinition>(request);
            
            definition.StatusCode.ShouldBe(HttpStatusCode.OK);
            definition.Data.Name.ShouldBe("demo SOx");
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
            var client = new RestClient($"https://{organization}.visualstudio.com/{{project}}/_apis/{{area}}/{{resource}}/");
            
            var request = new RestRequest(Method.GET);
            request
                .AddUrlSegment("project", "SOx-compliant-demo")
                .AddUrlSegment("area", "serviceendpoint")
                .AddUrlSegment("resource", "endpoints");

            request.AddHeader("authorization", GenerateAuthorizationHeader(token));
            var definition = client.Execute<JsonCollection<ServiceEndpoint>>(request);
            
            definition.StatusCode.ShouldBe(HttpStatusCode.OK);
            definition.Data.Value.ShouldContain(e => e.Name == "p02-prd-devautsox-deploy (SPN)");
        }

        private string GenerateAuthorizationHeader(string token)
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
