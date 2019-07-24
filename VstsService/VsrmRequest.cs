using System;
using System.Collections.Generic;

namespace SecurePipelineScan.VstsService
{
    public class VsrmRequest<TInput, TResponse> : VstsRequest<TInput, TResponse>
        where TResponse: new()
    {
        public VsrmRequest(string resource) : base(resource)
        {
        }
        
        public VsrmRequest(string resource, IDictionary<string, object> queryParams) : base(resource, queryParams)
        {
        }

        public override Uri BaseUri(string organization) => new Uri($"https://vsrm.dev.azure.com/{organization}/");
    }

    public class VsrmRequest<TResponse> : VsrmRequest<TResponse, TResponse>, IVstsRequest<TResponse>
        where TResponse: new()
    {
        public VsrmRequest(string resource) : base(resource)
        {
        }
        
        public VsrmRequest(string resource, IDictionary<string, object> queryParams) : base(resource, queryParams)
        {
        }
    }
}