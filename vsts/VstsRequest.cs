using System;
using vsts.Response;
using RestSharp;

namespace vsts
{
    public class VstsRestRequest<TResponse> : RestRequest, IVstsRestRequest<TResponse>
        where TResponse: new()
    {
        public VstsRestRequest(string uri, Method method) : base(uri, method)
        {
        }

        public Uri BaseUri(string organization)
        {
            return new System.Uri($"https://{organization}.visualstudio.com/");
        }
    }
}