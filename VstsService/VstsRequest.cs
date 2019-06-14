using System;
using System.Collections.Generic;

namespace SecurePipelineScan.VstsService
{
    public class VstsRequest<TInput, TResponse> : IVstsRequest<TInput, TResponse>
        where TResponse: new()
    {
        public string Resource { get; }
        public IDictionary<string, string> QueryParams { get; }

        public VstsRequest(string resource)
        {
            Resource = resource;
        }

        public VstsRequest(string resource, IDictionary<string, string> queryParams)
        {
            Resource = resource;
            QueryParams = queryParams;
        }

        public Uri BaseUri(string organization)
        {
            return new System.Uri($"https://dev.azure.com/{organization}/");
        }
    }

    public class VstsRequest<TResponse> : VstsRequest<TResponse, TResponse>, IVstsRequest<TResponse>
        where TResponse: new()
    {
        public VstsRequest(string resource) : base(resource)
        {
        }

        public VstsRequest(string resource, IDictionary<string, string> queryParams) : base(resource, queryParams)
        {
        }
    }
}