using SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.VstsService.Requests
{
    public static class MemberEntitlementManagement
    {
        public static IEnumerableRequest<UserEntitlement> UserEntitlements() => 
            new MemberEntitlementManagementRequest<UserEntitlement>("_apis/UserEntitlements").AsEnumerable();
    }
}