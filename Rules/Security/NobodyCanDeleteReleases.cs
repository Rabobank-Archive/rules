using System.Collections.Generic;
using SecurePipelineScan.VstsService;

namespace SecurePipelineScan.Rules.Security
{
    public class NobodyCanDeleteReleases : NobodyCanDeleteThisPipelineBase, IRule, IReconcile
    {
        public NobodyCanDeleteReleases(IVstsRestClient client) : base(client)
        {
        }

        protected override string NamespaceName => "DeleteReleases";
        protected override string PermissionsDisplayName => "Delete releases";
        protected override IEnumerable<int> AllowedPermissions => new[] 
        {
            PermissionId.NotSet,
            PermissionId.Deny,
            PermissionId.DenyInherited
        };
        protected override IEnumerable<string> IgnoredIdentitiesDisplayNames => new[]
        {
            "Project Collection Administrators",
        };

        string IRule.Description => "Nobody can delete releases";
        string IRule.Why => "To ensure auditability, no data should be deleted. Therefore, nobody should be able to delete releases.";
        string[] IReconcile.Impact => new[]
        {
            "For all application groups the 'Delete Releases' permission is set to Deny",
            "For all single users the 'Delete Releases' permission is set to Deny"
        };
    }
}