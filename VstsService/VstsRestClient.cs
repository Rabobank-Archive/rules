using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using RestSharp.Serializers.Newtonsoft.Json;
using SecurePipelineScan.VstsService.Response;
using RestRequest = RestSharp.RestRequest;

namespace SecurePipelineScan.VstsService
{
    public class VstsRestClient : IVstsRestClient
    {
        private readonly string authorization;
        private readonly string organization;
        private RestClient restClient;

        public VstsRestClient(string organization, string token)
        {
            this.authorization = GenerateAuthorizationHeader(token);
            this.organization = organization;
            restClient = new RestClient();
        }

        public TResponse Get<TResponse>(IVstsRestRequest<TResponse> request)
            where TResponse : new()
        {
            restClient.BaseUrl = request.BaseUri(organization);
            var wrapper = new RestRequest(request.Uri)
                .AddHeader("authorization", authorization);

            return restClient.Execute<TResponse>(wrapper).Data;
        }
        
        public TResponse Post<TResponse>(IVstsPostRequest<TResponse> request) where TResponse : new()
        {
            restClient.BaseUrl = request.BaseUri(organization);
            var wrapper = new RestRequest(request.Uri, Method.POST)
            {
                JsonSerializer = new NewtonsoftJsonSerializer(new JsonSerializer { ContractResolver = new CamelCasePropertyNamesContractResolver() })
            }.AddHeader("authorization", authorization)
             .AddJsonBody(request.Body);

            return restClient.Execute<TResponse>(wrapper).ThrowOnError().Data;
        }

        public void Delete(IVstsRestRequest request)
        {
            restClient.BaseUrl = request.BaseUri(organization);
            var wrapper = new RestRequest(request.Uri, Method.DELETE)
                .AddHeader("authorization", authorization);

            restClient.Execute(wrapper);
        }


        private static string GenerateAuthorizationHeader(string token)
        {
            string encoded = Base64Encode($":{token}");
            return ($"Basic {encoded}");
        }

        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}