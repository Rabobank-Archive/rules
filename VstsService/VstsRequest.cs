using System;

namespace SecurePipelineScan.VstsService
{
    public class VstsRestRequest<TResponse> : IVstsRestRequest<TResponse>
        where TResponse: new()
    {
        public string Uri { get; }

        public VstsRestRequest(string uri)
        {
            Uri = uri;
        }

        public Uri BaseUri(string organization)
        {
            return new System.Uri($"https://dev.azure.com/{organization}/");
        }
    }
}