using System;
using System.Collections.Generic;
using SecurePipelineScan.VstsService.Enumerators;

namespace SecurePipelineScan.VstsService
{
    public class VstsRequest<TInput, TResponse> : IVstsRequest<TInput, TResponse>
    {
        public string Resource { get; }
        public IDictionary<string, object> QueryParams { get; }

        public IDictionary<string, object> Headers { get; }

        public VstsRequest(string resource) : this(resource, new Dictionary<string, object>())
        {
        }

        public VstsRequest(string resource, IDictionary<string, object> queryParams) : this(resource, queryParams, new Dictionary<string, object>())
        {
        }

        public VstsRequest(string resource, IDictionary<string, object> queryParams, IDictionary<string, object> headers)
        {
            Resource = resource;
            QueryParams = queryParams;
            Headers = headers;
        }

        public virtual Uri BaseUri(string organization) => new Uri($"https://dev.azure.com/{organization}/");
    }

    public class VstsRequest<TResponse> : VstsRequest<TResponse, TResponse>, IVstsRequest<TResponse>
    {
        public VstsRequest(string resource) : base(resource)
        {
        }

        public VstsRequest(string resource, IDictionary<string, object> queryParams) : base(resource, queryParams)
        {
        }
        
        public IEnumerableRequest<TResponse> AsEnumerable() => 
            new EnumerableRequest<TResponse, MultipleEnumerator<TResponse>>(this);
    }
}