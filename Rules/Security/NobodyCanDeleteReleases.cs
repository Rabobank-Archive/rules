using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Permissions;
using SecurePipelineScan.VstsService.Requests;
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

        public const int PermissionBitDeleteReleasePipelines = 4;
        public const int PermissionBitAdministerReleasePermissions = 512;
        public const int PermissionBitDeleteReleases = 1024;

        [ExcludeFromCodeCoverage] string IRule.Description => "Nobody can delete releases (SOx)";
        [ExcludeFromCodeCoverage] string IRule.Link => "https://confluence.dev.somecompany.nl/x/9I8AD";

        [ExcludeFromCodeCoverage]
        string[] IReconcile.Impact => new[]
        {
            "For all security groups the 'Delete Releases' permission is set to Deny",
            "For all security groups the 'Delete Release Pipeline' permission is set to Deny",
            "For all security groups the 'Administer Release Permissions' permission is set to Deny"
        };

        public async Task<bool?> EvaluateAsync(string projectId, Response.ReleaseDefinition releasePipeline)
        {
            if (projectId == null)
                throw new ArgumentNullException(nameof(projectId));
            if (releasePipeline == null)
                throw new ArgumentNullException(nameof(releasePipeline));

            return await Permissions(projectId, releasePipeline.Id, releasePipeline.Path)
                .ValidateAsync()
                .ConfigureAwait(false);
        }

        public async Task ReconcileAsync(string projectId, string itemId)
        {
            if (projectId == null)
                throw new ArgumentNullException(nameof(projectId));
            if (itemId == null)
                throw new ArgumentNullException(nameof(itemId));

            var releasePipeline = await _client.GetAsync(ReleaseManagement.Definition(projectId, itemId))
                .ConfigureAwait(false);

            await Permissions(projectId, itemId, releasePipeline.Path)
                .SetToAsync(PermissionId.Deny)
                .ConfigureAwait(false);

        }

        private ManagePermissions Permissions(string projectId, string itemId, string itemPath) =>
            ManagePermissions
                .ForReleasePipeline(_client, projectId, itemId, itemPath)
                .Permissions(PermissionBitDeleteReleasePipelines, PermissionBitAdministerReleasePermissions, PermissionBitDeleteReleases)
                .Allow(PermissionId.NotSet, PermissionId.Deny, PermissionId.DenyInherited)
                .Ignore("Project Collection Administrators");
    }
}