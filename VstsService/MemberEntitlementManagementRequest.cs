using System;
using System.Collections.Generic;

namespace SecurePipelineScan.VstsService
{
    public class MemberEntitlementManagementRequest<TResponse> : IVstsRequest<TResponse> 
        where TResponse: new()
    {
        public MemberEntitlementManagementRequest(string resource, IDictionary<string, object> queryParams)
        {
            Resource = resource;
            QueryParams = queryParams;
        }

        public MemberEntitlementManagementRequest(string resource) : this(resource, new Dictionary<string, object>())
        {
        }

        public string Resource { get; }

        public IDictionary<string, object> QueryParams { get;  }

        public Uri BaseUri(string organization) => new Uri($"https://vsaex.dev.azure.com/{organization}/");
    }
}