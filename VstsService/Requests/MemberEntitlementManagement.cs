using Microsoft.AspNetCore.JsonPatch;
using SecurePipelineScan.VstsService.Response;
using System;
using System.Collections.Generic;

namespace SecurePipelineScan.VstsService.Requests
{
    public static class MemberEntitlementManagement
    {
        public static IVstsRequest<JsonPatchDocument, Entitlements<UserEntitlement>> PatchUserEntitlements(Guid entitlementId) =>
            new MemberEntitlementManagementRequest<JsonPatchDocument, Entitlements<UserEntitlement>>($"_apis/UserEntitlements/{entitlementId}",
                new Dictionary<string, object>
                {
                    ["api-version"] = "5.0-preview.2"
                },
                new Dictionary<string, object>
                {
                    ["Content-Type"] = "application/json-patch+json"
                });

        public static IEnumerableRequest<UserEntitlement> UserEntitlements() => 
            new MemberEntitlementManagementRequest<UserEntitlement>("_apis/UserEntitlements").AsEnumerable();
    }
}