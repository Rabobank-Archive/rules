using System.Collections.Generic;
using SecurePipelineScan.VstsService;

namespace SecurePipelineScan.Rules.Security
{
    public class NobodyCanDeleteBuilds : PipelineRuleBase, IRule, IReconcile
    {
        public NobodyCanDeleteBuilds(IVstsRestClient client) : base(client)
        {
        }

        protected override string NamespaceId => "33344d9c-fc72-4d6f-aba5-fa317101a7e9"; // build
        protected override int PermissionBit => 8; //Delete builds
        protected override IEnumerable<int> AllowedPermissions => new[] 
        {
            PermissionId.NotSet,
            PermissionId.Deny,
            PermissionId.DenyInherited
        };
        protected override IEnumerable<string> IgnoredIdentitiesDisplayNames => new[]
        {
            "Project Collection Administrators",
            "Project Collection Build Administrators",
            "Project Collection Service Accounts"
        };

        string IRule.Description => "Nobody can delete builds";
        string IRule.Why => "To ensure auditability, no data should be deleted. Therefore, nobody should be able to delete builds.";
        string[] IReconcile.Impact => new[]
        {
            "For all application groups the 'Delete Builds' permission is set to Deny",
            "For all single users the 'Delete Builds' permission is set to Deny"
        };
    }
}