using System.Collections.Generic;
using SecurePipelineScan.VstsService;

namespace SecurePipelineScan.Rules.Security
{
    public class NobodyCanDeleteReleasePipelines : PipelineRuleBase, IRule, IReconcile
    {
        public NobodyCanDeleteReleasePipelines(IVstsRestClient client) : base(client)
        {
        }

        protected override string NamespaceId => "c788c23e-1b46-4162-8f5e-d7585343b5de"; // release management
        protected override int PermissionBit => 4; //Delete release pipeline
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