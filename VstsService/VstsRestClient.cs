using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using RestSharp;
using RestSharp.Serializers.Newtonsoft.Json;
using SecurePipelineScan.VstsService.Converters;
using RestRequest = RestSharp.RestRequest;

namespace SecurePipelineScan.VstsService
{
    public class VstsRestClient : IVstsRestClient
    {
        private readonly string _authorization;
        private readonly string _organization;
        private readonly IRestClient _client;

        public VstsRestClient(string organization, string token, IRestClient client)
        {
            _client = client.SetupSerializer();

            _authorization = GenerateAuthorizationHeader(token);
            _organization = organization;
        }

        public VstsRestClient(string organization, string token) : this(organization, token, new RestClient())
        {
        }

        public TResponse Get<TResponse>(IVstsRestRequest<TResponse> request)
            where TResponse : new()
        {
            _client.BaseUrl = request.BaseUri(_organization);
            var wrapper = new RestRequest(request.Uri)
                .AddHeader("authorization", _authorization);

            if (request is IVstsRestRequest<JObject>)
            {
                return (TResponse) (object) JObject.Parse(_client.Execute(wrapper).ThrowOnError().Content);
            }

            return _client.Execute<TResponse>(wrapper).ThrowOnError().DefaultIfNotFound();
        }

        public TResponse Post<TInput, TResponse>(IVstsPostRequest<TInput, TResponse> request, TInput body) where TResponse : new()
        {
            _client.BaseUrl = request.BaseUri(_organization);
            var wrapper = new RestRequest(request.Uri, Method.POST)
            {
                JsonSerializer = new NewtonsoftJsonSerializer(new JsonSerializer { ContractResolver = new CamelCasePropertyNamesContractResolver(), Converters = { new PolicyConverter()}})
            }.AddHeader("authorization", _authorization)
             .AddJsonBody(body);

            return _client.Execute<TResponse>(wrapper).ThrowOnError().Data;
        }

        public TResponse Put<TResponse>(IVstsRestRequest<TResponse> request, TResponse body) where TResponse: new()
        {
            _client.BaseUrl = request.BaseUri(_organization);
            var wrapper = new RestRequest(request.Uri, Method.PUT)
            {
                JsonSerializer = new NewtonsoftJsonSerializer(new JsonSerializer { ContractResolver = new CamelCasePropertyNamesContractResolver() })
            }.AddHeader("authorization", _authorization)
             .AddJsonBody(body);

            return _client.Execute<TResponse>(wrapper).ThrowOnError().Data;
        }

        public void Delete(IVstsRestRequest request)
        {
            _client.BaseUrl = request.BaseUri(_organization);
            var wrapper = new RestRequest(request.Uri, Method.DELETE)
                .AddHeader("authorization", _authorization);

            _client.Execute(wrapper).ThrowOnError();
        }

        private static string GenerateAuthorizationHeader(string token)
        {
            var encoded = Base64Encode($":{token}");
            return ($"Basic {encoded}");
        }

        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}