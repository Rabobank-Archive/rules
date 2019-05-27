
using System;

namespace SecurePipelineScan.VstsService
{
    public class ExtmgmtRequest<TResponse> : IVstsRequest<TResponse>
        where TResponse: new()
    {
        public string Uri { get; }

        public ExtmgmtRequest(string uri)
        {
            Uri = uri;
        }

        public Uri BaseUri(string organization)
        {
            return new System.Uri($"https://{organization}.extmgmt.visualstudio.com/");
        }
    }
}