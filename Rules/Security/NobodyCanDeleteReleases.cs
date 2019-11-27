using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using SecurePipelineScan.Rules.Permissions;
using SecurePipelineScan.VstsService;
using Response = SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.Rules.Security
{
    public class NobodyCanDeleteReleases : IReleasePipelineRule, IReconcile
    {
        private readonly IVstsRestClient _client;

        public NobodyCanDeleteReleases(IVstsRestClient client)
        {
            _client = client;
        }

        private const int PermissionBitDeleteReleasePipelines = 4;
        private const int PermissionBitAdministerReleasePermissions = 512;
        private const int PermissionBitDeleteReleases = 1024;

        [ExcludeFromCodeCoverage]string IRule.Description => "Nobody can delete releases (SOx)";
        [ExcludeFromCodeCoverage]string IRule.Link => "https://confluence.dev.somecompany.nl/x/9I8AD";
        [ExcludeFromCodeCoverage]bool IRule.IsSox => true;
        [ExcludeFromCodeCoverage]bool IReconcile.RequiresStageId => false;
        [ExcludeFromCodeCoverage]string[] IReconcile.Impact => new[]
        {
            "For all security groups the 'Delete Releases' permission is set to Deny",
            "For all security groups the 'Delete Release Pipeline' permission is set to Deny",
            "For all security groups the 'Administer Release Permissions' permission is set to Deny"
        };

        public async Task<bool?> EvaluateAsync(string projectId, string stageId, Response.ReleaseDefinition releasePipeline)
        {
            if (projectId == null)
                throw new ArgumentNullException(nameof(projectId));
            if (releasePipeline == null)
                throw new ArgumentNullException(nameof(releasePipeline));

            return await Permissions(projectId, releasePipeline.Id)
                .ValidateAsync()
                .ConfigureAwait(false);
        }

        public Task ReconcileAsync(string projectId, string itemId, string stageId)
        {
            if (projectId == null)
                throw new ArgumentNullException(nameof(projectId));
            if (itemId == null)
                throw new ArgumentNullException(nameof(itemId));

            return Permissions(projectId, itemId)
                .SetToAsync(PermissionId.Deny);
        }

        private ManagePermissions Permissions(string projectId, string itemId) =>
            ManagePermissions
                .ForReleasePipeline(_client, projectId, itemId)
                .Permissions(PermissionBitDeleteReleasePipelines, PermissionBitAdministerReleasePermissions, PermissionBitDeleteReleases)
                .Allow(PermissionId.NotSet, PermissionId.Deny, PermissionId.DenyInherited)
                .Ignore("Project Collection Administrators");
    }
}