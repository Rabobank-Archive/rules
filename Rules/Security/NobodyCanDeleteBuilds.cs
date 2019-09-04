using System.Collections.Generic;
using SecurePipelineScan.VstsService;

namespace SecurePipelineScan.Rules.Security
{
    public class NobodyCanDeleteBuilds : PipelineHasPermissionRuleBase, IRule, IReconcile
    {
        public NobodyCanDeleteBuilds(IVstsRestClient client) : base(client)
        {
            //nothing
        }

        const int PermissionBitDeleteBuilds = 8;
        const int PermissionBitDestroyBuilds = 32;
        const int PermissionBitDeleteBuildDefinition = 4096;
        const int PermissionBitAdministerBuildPermissions = 16384;
        
        protected override string NamespaceId => "33344d9c-fc72-4d6f-aba5-fa317101a7e9"; //build
        protected override IEnumerable<int> PermissionBits => new[]
        {
            PermissionBitDeleteBuilds,
            PermissionBitDestroyBuilds,
            PermissionBitDeleteBuildDefinition,
            PermissionBitAdministerBuildPermissions
        };
        protected override IEnumerable<int> AllowedPermissions => new[] 
        {
            PermissionId.NotSet,
            PermissionId.Deny,
            PermissionId.DenyInherited
        };
        protected override IEnumerable<string> IgnoredIdentitiesDisplayNames => new[]
        {
            "Project Collection Administrators",
            "Project Collection Build Administrators"
        };

        string IRule.Description => "Nobody can delete builds";
        string IRule.Why => "To ensure auditability, no data should be deleted. " +
            "Therefore, nobody should be able to delete build runs.";
        bool IRule.IsSox => true;
        string[] IReconcile.Impact => new[]
        {
            "For all security groups the 'Delete Builds' permission is set to Deny",
            "For all security groups the 'Destroy Builds' permission is set to Deny",
            "For all security groups the 'Delete Build Definitions' permission is set to Deny",
            "For all security groups the 'Administer Build Permissions' permission is set to Deny"
        };
    }
}