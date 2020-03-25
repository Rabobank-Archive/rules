using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using AzureDevOps.Compliance.Rules.PermissionBits;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Permissions;

namespace AzureDevOps.Compliance.Rules
{
    public sealed class NobodyCanBypassPolicies : IRepositoryRule, IReconcile
    {
        private readonly IVstsRestClient _client;

        public NobodyCanBypassPolicies(IVstsRestClient client) => _client = client;

        [ExcludeFromCodeCoverage] string IRule.Description => "Nobody can bypass branch policies";
        [ExcludeFromCodeCoverage] string IRule.Link => "https://github.com/azure-devops-compliance/rules/wiki/Rules-NobodyCanBypassPolicies";

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

        private ManagePermissions PermissionsRepository(string projectId, string repositoryId) => 
            Permissions(ManagePermissions.ForRepository(_client, projectId, repositoryId));

        private ManagePermissions PermissionsMasterBranch(string projectId, string repositoryId) =>
            Permissions(ManagePermissions.ForMasterBranch(_client, projectId, repositoryId));

        private static ManagePermissions Permissions(ManagePermissions manage) =>
            manage
                .Permissions(Repository.BypassPoliciesPullRequest, Repository.BypassPoliciesCodePush, Repository.ManagePermissions)
                .Allow(PermissionId.NotSet, PermissionId.Deny, PermissionId.DenyInherited)
                .Ignore("Project Collection Administrators", "Project Collection Service Accounts");
    }
}