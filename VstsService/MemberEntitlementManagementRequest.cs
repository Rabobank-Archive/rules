using System;
using System.Collections.Generic;

namespace SecurePipelineScan.VstsService
{
    public class MemberEntitlementManagementRequest<TResponse> : VstsRequest<TResponse> 
        where TResponse: new()
    {
        public MemberEntitlementManagementRequest(string resource, IDictionary<string, object> queryParams) : base(resource, queryParams)
        {
        }

        public MemberEntitlementManagementRequest(string resource) : base(resource) 
        {
        }

        public override Uri BaseUri(string organization) => new Uri($"https://vsaex.dev.azure.com/{organization}/");
    }
}