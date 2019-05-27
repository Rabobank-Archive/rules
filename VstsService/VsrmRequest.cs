using System;

namespace SecurePipelineScan.VstsService
{
    public class VsrmRequest<TInput, TResponse> : IVstsRequest<TInput, TResponse>
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

    public class VsrmRequest<TResponse> : VsrmRequest<TResponse, TResponse>, IVstsRequest<TResponse>
        where TResponse: new()
    {
        public VsrmRequest(string uri) : base(uri)
        {
        }
    }
}