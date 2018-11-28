using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using RestSharp.Serializers.Newtonsoft.Json;
using RestRequest = RestSharp.RestRequest;

namespace SecurePipelineScan.VstsService
{
    public class VstsRestRequest<TResponse> : RestRequest, IVstsRestRequest<TResponse>
        where TResponse: new()
    {
        public VstsRestRequest(string uri, Method method) : base(uri, method)
        {
        }

        public VstsRestRequest(string uri, Method method, object body) : this(uri, method)
        {
            JsonSerializer = new NewtonsoftJsonSerializer(new JsonSerializer { ContractResolver = new CamelCasePropertyNamesContractResolver() });
            AddJsonBody(body);
        }

        public Uri BaseUri(string organization)
        {
            return new System.Uri($"https://dev.azure.com/{organization}/");
        }
    }
}