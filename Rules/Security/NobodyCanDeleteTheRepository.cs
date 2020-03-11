using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using SecurePipelineScan.Rules.PermissionBits;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Permissions;

namespace SecurePipelineScan.Rules.Security
{
    public sealed class NobodyCanDeleteTheRepository : IRepositoryRule, IReconcile
    {
        private readonly IVstsRestClient _client;

        public NobodyCanDeleteTheRepository(IVstsRestClient client) => _client = client;
        
        [ExcludeFromCodeCoverage] string IRule.Description => "Nobody can delete the repository (SOx)";
        [ExcludeFromCodeCoverage] string IRule.Link => "https://confluence.dev.somecompany.nl/x/RI8AD";

        [ExcludeFromCodeCoverage]
        string[] IReconcile.Impact => new[]
        {
            "For all security groups the 'Delete Repository' permission is set to Deny",
            "For all security groups the 'Manage Permissions' permission is set to Deny"
        };

        public Task<bool> EvaluateAsync(string projectId, string repositoryId)
        {
            if (projectId == null)
                throw new ArgumentNullException(nameof(projectId));
            if (repositoryId == null)
                throw new ArgumentNullException(nameof(repositoryId));

            return Permissions(projectId, repositoryId)
                .ValidateAsync();
        }

        public Task ReconcileAsync(string projectId, string itemId)
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
                .ForRepository(_client, projectId, itemId)
                .Permissions(Repository.DeleteRepository, Repository.ManagePermissions)
                .Allow(PermissionId.NotSet, PermissionId.Deny, PermissionId.DenyInherited)
                .Ignore("Project Collection Administrators", "Project Collection Service Accounts");
    }
}