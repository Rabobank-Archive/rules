using System;
using lib.Response;
using RestSharp;

namespace lib.Vsts
{
    public class VstsRequest<TResponse> : RestRequest, IVstsRequest<TResponse>
        where TResponse: new()
    {
        public VstsRequest(string uri, Method method) : base(uri, method)
        {
        }

        public Uri BaseUri(string organization)
        {
            return new System.Uri($"https://{organization}.visualstudio.com/");
        }
    }
}