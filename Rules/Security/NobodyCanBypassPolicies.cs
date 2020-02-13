using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Permissions;

namespace SecurePipelineScan.Rules.Security
{
    public sealed class NobodyCanBypassPolicies : IRepositoryRule, IReconcile
    {
        private readonly IVstsRestClient _client;

        public NobodyCanBypassPolicies(IVstsRestClient client) => _client = client;

        private const int PermissionBitBypassPoliciesPullRequest = 32768;
        private const int PermissionBitBypassPoliciesCodePush = 128;
        private const int PermissionBitManagePermissions = 8192;

        [ExcludeFromCodeCoverage] string IRule.Description => "Nobody can bypass branch policies (SOx)";
        [ExcludeFromCodeCoverage] string IRule.Link => "https://confluence.dev.somecompany.nl/x/fRN7DQ";

        [ExcludeFromCodeCoverage]
        string[] IReconcile.Impact => new[]
        {
            "For all security groups the 'Bypass policies when completing pull requests' permission is set to Deny",
            "For all security groups the 'Bypass policies when pushing' permission is set to Deny",
            "For all security groups the 'Manage Permissions' permission is set to Deny"
        };

        public async Task<bool> EvaluateAsync(string projectId, string repositoryId)
        {
            if (projectId == null)
                throw new ArgumentNullException(nameof(projectId));
            if (repositoryId == null)
                throw new ArgumentNullException(nameof(repositoryId));

            return await PermissionsRepository(projectId, repositoryId)
                    .ValidateAsync()
                    .ConfigureAwait(false) &&
                await PermissionsMasterBranch(projectId, repositoryId)
                    .ValidateAsync()
                    .ConfigureAwait(false);
        }

        public async Task ReconcileAsync(string projectId, string itemId)
        {
            if (projectId == null)
                throw new ArgumentNullException(nameof(projectId));
            if (itemId == null)
                throw new ArgumentNullException(nameof(itemId));

            await PermissionsRepository(projectId, itemId)
                .SetToAsync(PermissionId.Deny)
                .ConfigureAwait(false);
            await PermissionsMasterBranch(projectId, itemId)
                .SetToAsync(PermissionId.Deny)
                .ConfigureAwait(false);
        }

        private ManagePermissions PermissionsRepository(string projectId, string repositoryId)
        {
            var manage = ManagePermissions
                .ForRepository(_client, projectId, repositoryId);
            return Permissions(manage);
        }

        private ManagePermissions PermissionsMasterBranch(string projectId, string repositoryId)
        {
            var manage = ManagePermissions
                .ForMasterBranch(_client, projectId, repositoryId);
            return Permissions(manage);
        }

        private static ManagePermissions Permissions(ManagePermissions response)
        {
            return response
                .Permissions(PermissionBitBypassPoliciesPullRequest, PermissionBitBypassPoliciesCodePush,
                    PermissionBitManagePermissions)
                .Allow(PermissionId.NotSet, PermissionId.Deny, PermissionId.DenyInherited)
                .Ignore("Project Collection Administrators", "Project Collection Service Accounts");
        }
    }
}