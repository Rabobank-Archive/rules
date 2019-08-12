using System.Collections.Generic;
using SecurePipelineScan.VstsService;

namespace SecurePipelineScan.Rules.Security
{
    public class NobodyCanDeleteReleases : PipelineRuleBase, IRule, IReconcile
    {
        public NobodyCanDeleteReleases(IVstsRestClient client) : base(client)
        {
            //nothing
        }

        const int PermissionBitDeleteReleasePipelines = 4;
        const int PermissionBitAdministerReleasePermissions = 512;
        const int PermissionBitDeleteReleases = 1024;

        protected override string NamespaceId => "c788c23e-1b46-4162-8f5e-d7585343b5de"; //release management
        protected override IEnumerable<int> PermissionBits => new[]
        {
            PermissionBitDeleteReleasePipelines,
            PermissionBitAdministerReleasePermissions,
            PermissionBitDeleteReleases
        };
        protected override IEnumerable<int> AllowedPermissions => new[]
        {
            PermissionId.NotSet,
            PermissionId.Deny,
            PermissionId.DenyInherited
        };
        protected override IEnumerable<string> IgnoredIdentitiesDisplayNames => new[]
        {
            "Project Collection Administrators"
        };

        string IRule.Description => "Nobody can delete releases";
        string IRule.Why => "To ensure auditability, no data should be deleted. " +
            "Therefore, nobody should be able to delete releases.";
        bool IRule.isSox => true;
        string[] IReconcile.Impact => new[]
        {
            "For all security groups the 'Delete Releases' permission is set to Deny",
            "For all security groups the 'Delete Release Pipeline' permission is set to Deny",
            "For all security groups the 'Administer Release Permissions' permission is set to Deny"
        };
    }
}