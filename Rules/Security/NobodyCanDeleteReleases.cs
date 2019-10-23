using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SecurePipelineScan.VstsService;
using Response = SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.Rules.Security
{
    public class NobodyCanDeleteReleases : ItemHasPermissionRuleBase, IReleasePipelineRule, IReconcile
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
        string IRule.Link => "https://confluence.dev.somecompany.nl/x/9I8AD";
        bool IRule.IsSox => true;

        string[] IReconcile.Impact => new[]
        {
            "For all security groups the 'Delete Releases' permission is set to Deny",
            "For all security groups the 'Delete Release Pipeline' permission is set to Deny",
            "For all security groups the 'Administer Release Permissions' permission is set to Deny"
        };

        public async Task<bool> EvaluateAsync(string projectId, string stageId, Response.ReleaseDefinition releasePipeline)
        {
            if (projectId == null)
                throw new ArgumentNullException(nameof(projectId));
            if (releasePipeline == null)
                throw new ArgumentNullException(nameof(releasePipeline));

            return await base.EvaluateAsync(projectId, releasePipeline.Id, RuleScopes.ReleasePipelines)
                .ConfigureAwait(false);
        }

        public async Task ReconcileAsync(string projectId, string itemId)
        {
            if (projectId == null)
                throw new ArgumentNullException(nameof(projectId));
            if (itemId == null)
                throw new ArgumentNullException(nameof(itemId));

            await ReconcileAsync(projectId, itemId, RuleScopes.ReleasePipelines)
                .ConfigureAwait(false);
        }
    }
}