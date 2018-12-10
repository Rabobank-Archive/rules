
using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using RestSharp.Serializers.Newtonsoft.Json;
using RestRequest = RestSharp.RestRequest;

namespace SecurePipelineScan.VstsService
{
    public class VsrmRequest<TResponse> : IVstsRestRequest<TResponse>
        where TResponse: new()
    {
        public string Uri { get; }

        public VsrmRequest(string uri)
        {
            Uri = uri;
        }

        public Uri BaseUri(string organization)
        {
            return new System.Uri($"https://vsrm.dev.azure.com/{organization}/");
        }
    }
}