using System;

namespace SecurePipelineScan.VstsService
{
    public class VstsRequest<TInput, TResponse> : IVstsRequest<TInput, TResponse>
        where TResponse: new()
    {
        public string Uri { get; }

        public VstsRequest(string uri)
        {
            Uri = uri;
        }

        public Uri BaseUri(string organization)
        {
            return new System.Uri($"https://dev.azure.com/{organization}/");
        }
    }

    public class VstsRequest<TResponse> : VstsRequest<TResponse, TResponse>, IVstsRequest<TResponse>
        where TResponse: new()
    {
        public VstsRequest(string uri) : base(uri)
        {
        }
    }
}