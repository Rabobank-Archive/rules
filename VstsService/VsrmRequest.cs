using System;
using System.Collections.Generic;

namespace SecurePipelineScan.VstsService
{
    public class VsrmRequest<TInput, TResponse> : IVstsRequest<TInput, TResponse>
        where TResponse: new()
    {
        public string Resource { get; }
        public IDictionary<string, string> QueryParams { get; }

        public VsrmRequest(string resource)
        {
            Resource = resource;
        }
        
        public VsrmRequest(string resource, IDictionary<string, string> queryParams)
        {
            Resource = resource;
            QueryParams = queryParams;
        }

        public Uri BaseUri(string organization)
        {
            return new System.Uri($"https://vsrm.dev.azure.com/{organization}/");
        }
    }

    public class VsrmRequest<TResponse> : VsrmRequest<TResponse, TResponse>, IVstsRequest<TResponse>
        where TResponse: new()
    {
        public VsrmRequest(string resource) : base(resource)
        {
        }
        
        public VsrmRequest(string resource, IDictionary<string, string> queryParams) : base(resource, queryParams)
        {
        }
    }
}