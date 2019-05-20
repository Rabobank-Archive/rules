using System.Collections.Generic;
using SecurePipelineScan.VstsService;

namespace SecurePipelineScan.Rules.Security
{
    public class NobodyCanDeleteBuildPipelines : PipelineRuleBase, IRule, IReconcile
    {
        public NobodyCanDeleteBuildPipelines(IVstsRestClient client) : base(client)
        {
        }

        protected override string NamespaceId => "33344d9c-fc72-4d6f-aba5-fa317101a7e9"; // build
        protected override string PermissionsDisplayName => "Delete build definition";
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

        string IRule.Description => "Nobody can delete build pipelines";
        string IRule.Why => "To ensure auditability, no data should be deleted. Therefore, nobody should be able to delete build pipelines.";
        string[] IReconcile.Impact => new[]
        {
            "For all application groups the 'Delete Build Definitions' permission is set to Deny",
            "For all single users the 'Delete Build Definitions' permission is set to Deny"
        };
    }
}