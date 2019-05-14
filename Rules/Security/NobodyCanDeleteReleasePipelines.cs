using System.Collections.Generic;
using SecurePipelineScan.VstsService;

namespace SecurePipelineScan.Rules.Security
{
    public class NobodyCanDeleteReleasePipelines : NobodyCanDeleteThisPipelineBase, IRule, IReconcile
    {
        public NobodyCanDeleteReleasePipelines(IVstsRestClient client) : base(client)
        {
        }

        protected override string NamespaceName => "DeleteReleaseDefinition";
        protected override string PermissionsDisplayName => "Delete release pipeline";
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

        string IRule.Description => "Nobody can delete release pipelines";
        string IRule.Why => "To ensure auditability, no data should be deleted. Therefore, nobody should be able to delete release pipelines.";
        string[] IReconcile.Impact => new[]
        {
            "For all application groups the 'Delete Release Pipeline' permission is set to Deny",
            "For all single users the 'Delete Release Pipeline' permission is set to Deny"
        };
    }
}