using System;
using System.Collections.Generic;
using SecurePipelineScan.VstsService.Enumerators;

namespace SecurePipelineScan.VstsService
{
    public class MemberEntitlementManagementRequest<TResponse> : VstsRequest<TResponse> 
    {
        public MemberEntitlementManagementRequest(string resource, IDictionary<string, object> queryParams) : base(resource, queryParams)
        {
        }

        public MemberEntitlementManagementRequest(string resource) : base(resource) 
        {
        }

        public override Uri BaseUri(string organization) => new Uri($"https://vsaex.dev.azure.com/{organization}/");
        
        public IEnumerableRequest<TResponse> AsEnumerable() => 
            new EnumerableRequest<TResponse, MemberEntitlementsEnumerator<TResponse>>(this);
    }

    public class MemberEntitlementManagementRequest<TInput, TResponse> : VstsRequest<TInput, TResponse>
        where TResponse : new()
    {
        public MemberEntitlementManagementRequest(string resource, Dictionary<string, object> queryParams, IDictionary<string, object> headers) : base(resource, queryParams, headers)
        {
        }

        public MemberEntitlementManagementRequest(string resource) : base(resource)
        {
        }

        public override Uri BaseUri(string organization) => new Uri($"https://vsaex.dev.azure.com/{organization}/");
    }
}