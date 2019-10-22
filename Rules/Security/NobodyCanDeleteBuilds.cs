using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SecurePipelineScan.VstsService;
using Response = SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.Rules.Security
{
    public class NobodyCanDeleteBuilds : ItemHasPermissionRuleBase, IBuildPipelineRule, IReconcile
    {
        public NobodyCanDeleteBuilds(IVstsRestClient client) : base(client)
        {
            //nothing
        }

        private const int PermissionBitDeleteBuilds = 8;
        private const int PermissionBitDestroyBuilds = 32;
        private const int PermissionBitDeleteBuildDefinition = 4096;
        private const int PermissionBitAdministerBuildPermissions = 16384;

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
        string IRule.Link => "https://confluence.dev.somecompany.nl/x/V48AD";
        bool IRule.IsSox => true;

        string[] IReconcile.Impact => new[]
        {
            "For all security groups the 'Delete Builds' permission is set to Deny",
            "For all security groups the 'Destroy Builds' permission is set to Deny",
            "For all security groups the 'Delete Build Definitions' permission is set to Deny",
            "For all security groups the 'Administer Build Permissions' permission is set to Deny"
        };

        public async Task<bool?> EvaluateAsync(string projectId, Response.BuildDefinition buildPipeline)
        {
            if (projectId == null)
                throw new ArgumentNullException(nameof(projectId));
            if (buildPipeline == null)
                throw new ArgumentNullException(nameof(buildPipeline));

            return await base.EvaluateAsync(projectId, buildPipeline.Id, RuleScopes.BuildPipelines)
                .ConfigureAwait(false);
        }

        public async Task ReconcileAsync(string projectId, string itemId)
        {
            if (projectId == null)
                throw new ArgumentNullException(nameof(projectId));
            if (itemId == null)
                throw new ArgumentNullException(nameof(itemId));

            await ReconcileAsync(projectId, itemId, RuleScopes.BuildPipelines)
                .ConfigureAwait(false);
        }
    }
}