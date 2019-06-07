using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using SecurePipelineScan.VstsService.Converters;
using SecurePipelineScan.VstsService.Requests;
using SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.VstsService
{
    public class VstsRestClient : IVstsRestClient
    {
        private readonly string _authorization;
        private readonly string _organization;
        private readonly IRestClientFactory _factory;

        public VstsRestClient(string organization, string token, IRestClientFactory factory)
        {
            _authorization = GenerateAuthorizationHeader(token);
            _organization = organization;
            _factory = factory;
        }

        internal VstsRestClient(string organization, string token) : this(organization, token, new RestClientFactory())
        {   
        }

        public TResponse Get<TResponse>(IVstsRequest<TResponse> request)
            where TResponse : new()
        {
            var client = _factory.Create(request.BaseUri(_organization));
            var wrapper = new RestRequest(request.Uri)
                .AddHeader("authorization", _authorization);

            if (request is IVstsRequest<JObject>)
            {
                return (TResponse) (object) JObject.Parse(client.Execute(wrapper).ThrowOnError().Content);
            }
            
            return client.Execute<TResponse>(wrapper)
                .ThrowOnError()
                .DefaultIfNotFound();
        }

        public IEnumerable<TResponse> Get<TResponse>(IVstsRequest<Multiple<TResponse>> request) where TResponse : new()
        {
            var client = _factory.Create(request.BaseUri(_organization));
            var wrapper = new RestRequest(request.Uri)
                .AddHeader("authorization", _authorization);

            return new MultipleEnumerator<TResponse>(wrapper, client);
        }

        public TResponse Post<TInput, TResponse>(IVstsRequest<TInput, TResponse> request, TInput body) where TResponse : new()
        {
            var client = _factory.Create(request.BaseUri(_organization));
            var wrapper = new RestRequest(request.Uri, HttpMethod.Post)
                .AddHeader("authorization", _authorization)
                .AddJsonBody(body);

            return client.Execute<TResponse>(wrapper).ThrowOnError().Data;
        }

        public TResponse Put<TInput, TResponse>(IVstsRequest<TInput, TResponse> request, TInput body) where TResponse: new()
        {
            var client = _factory.Create(request.BaseUri(_organization));
            var wrapper = new RestRequest(request.Uri, HttpMethod.Put)
                .AddHeader("authorization", _authorization)
                .AddJsonBody(body);

            return client.Execute<TResponse>(wrapper).ThrowOnError().Data;
        }

        public void Delete(IVstsRequest request)
        {
            var client = _factory.Create(request.BaseUri(_organization));
            var wrapper = new RestRequest(request.Uri, HttpMethod.Delete)
                .AddHeader("authorization", _authorization);

            client.Execute(wrapper).ThrowOnError();
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