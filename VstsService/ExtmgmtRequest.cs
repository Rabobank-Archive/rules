
using System;
using System.Collections.Generic;

namespace SecurePipelineScan.VstsService
{
    public class ExtmgmtRequest<TResponse> : IVstsRequest<TResponse>
        where TResponse: new()
    {
        public string Resource { get; }
        public IDictionary<string, string> QueryParams { get; }

        public ExtmgmtRequest(string resource)
        {
            Resource = resource;
        }

        public ExtmgmtRequest(string resource, IDictionary<string, string> queryParams)
        {
            Resource = resource;
            QueryParams = queryParams;
        }

        public Uri BaseUri(string organization)
        {
            return new System.Uri($"https://{organization}.extmgmt.visualstudio.com/");
        }
    }
}