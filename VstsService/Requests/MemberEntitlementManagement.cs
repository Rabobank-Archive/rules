using SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.VstsService.Requests
{
    public static class MemberEntitlementManagement
    {
        public static MemberEntitlementManagementRequest<Entitlements<UserEntitlement>> UserEntitlements()
        {
            return new MemberEntitlementManagementRequest<Entitlements<UserEntitlement>>("_apis/UserEntitlements");
        }
    }
}