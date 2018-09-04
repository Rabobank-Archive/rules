using System;
using lib.Response;
using RestSharp;

namespace lib.Vsts
{
    public class VsrmRequest<TResponse> : RestRequest, IVstsRestRequest<TResponse>
        where TResponse: new()
    {
        public VsrmRequest(string uri, Method method) : base(uri, method)
        {
        }

        public Uri BaseUri(string organization)
        {
            return new System.Uri($"https://{organization}.vsrm.visualstudio.com/");
        }
    }
}