
using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using RestSharp.Serializers.Newtonsoft.Json;
using RestRequest = RestSharp.RestRequest;

namespace SecurePipelineScan.VstsService
{
    public class VsrmRequest<TResponse> : RestRequest, IVstsRestRequest<TResponse>
        where TResponse: new()
    {
        public VsrmRequest(string uri, Method method) : base(uri, method)
        {
        }

        public VsrmRequest(string uri, Method method, object body) : this(uri, method)
        {
            JsonSerializer = new NewtonsoftJsonSerializer(new JsonSerializer { ContractResolver = new CamelCasePropertyNamesContractResolver() });
            AddJsonBody(body);
        }

        public Uri BaseUri(string organization)
        {
            return new System.Uri($"https://vsrm.dev.azure.com/{organization}/");
        }
    }
}