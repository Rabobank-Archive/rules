using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using SecurePipelineScan.Rules.Permissions;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Requests;
using Response = SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.Rules.Security
{
    public class NobodyCanDeleteBuilds : IBuildPipelineRule, IReconcile
    {
        private readonly IVstsRestClient _client;

        public NobodyCanDeleteBuilds(IVstsRestClient client) => _client = client;

        private const int PermissionBitDeleteBuilds = 8;
        private const int PermissionBitDestroyBuilds = 32;
        private const int PermissionBitDeleteBuildDefinition = 4096;
        private const int PermissionBitAdministerBuildPermissions = 16384;
        [ExcludeFromCodeCoverage] string IRule.Description => "Nobody can delete builds (SOx)";
        [ExcludeFromCodeCoverage] string IRule.Link => "https://confluence.dev.somecompany.nl/x/V48AD";

        [ExcludeFromCodeCoverage]
        string[] IReconcile.Impact => new[]
        {
            "For all security groups the 'Delete Builds' permission is set to Deny",
            "For all security groups the 'Destroy Builds' permission is set to Deny",
            "For all security groups the 'Delete Build Definitions' permission is set to Deny",
            "For all security groups the 'Administer Build Permissions' permission is set to Deny"
        };

        public async Task<bool?> EvaluateAsync(Response.Project project, Response.BuildDefinition buildPipeline)
        {
            if (project == null)
                throw new ArgumentNullException(nameof(project));
            if (buildPipeline == null)
                throw new ArgumentNullException(nameof(buildPipeline));

            return await Permissions(project.Id, buildPipeline.Id, buildPipeline.Path)
                .ValidateAsync()
                .ConfigureAwait(false);
        }

        public async Task ReconcileAsync(string projectId, string itemId)
        {
            if (projectId == null)
                throw new ArgumentNullException(nameof(projectId));
            if (itemId == null)
                throw new ArgumentNullException(nameof(itemId));

            var buildPipeline = await _client.GetAsync(Builds.BuildDefinition(projectId, itemId))
                .ConfigureAwait(false);

            await Permissions(projectId, itemId, buildPipeline.Path)
                .SetToAsync(PermissionId.Deny)
                .ConfigureAwait(false);
        }

        private ManagePermissions Permissions(string projectId, string itemId, string itemPath) =>
            ManagePermissions
                .ForBuildPipeline(_client, projectId, itemId, itemPath)
                .Ignore("Project Collection Administrators", "Project Collection Build Administrators")
                .Permissions(PermissionBitDeleteBuilds, PermissionBitDestroyBuilds, PermissionBitDeleteBuildDefinition, PermissionBitAdministerBuildPermissions)
                .Allow(PermissionId.NotSet, PermissionId.Deny, PermissionId.DenyInherited);
    }
}

