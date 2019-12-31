using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using SecurePipelineScan.Rules.Permissions;
using SecurePipelineScan.VstsService;
using Response = SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.Rules.Security
{
    public sealed class NobodyCanBypassPolicies : IRepositoryRule, IReconcile
    {
        private readonly IVstsRestClient _client;

        public NobodyCanBypassPolicies(IVstsRestClient client) => _client = client;

        private const int PermissionBitBypassPoliciesPullRequest = 32768;
        private const int PermissionBitBypassPoliciesCodePush = 128;
        private const int PermissionBitManagePermissions = 8192;

        [ExcludeFromCodeCoverage]string IRule.Description => "Nobody can bypass branch policies (SOx)";
        [ExcludeFromCodeCoverage]string IRule.Link => "https://confluence.dev.somecompany.nl/x/fRN7DQ";
        [ExcludeFromCodeCoverage]bool IRule.IsSox => true;
        [ExcludeFromCodeCoverage]bool IReconcile.RequiresStageId => false;
        [ExcludeFromCodeCoverage]string[] IReconcile.Impact => new[]
        {
            "For all security groups the 'Bypass policies when completing pull requests' permission is set to Deny",
            "For all security groups the 'Bypass policies when pushing' permission is set to Deny",
            "For all security groups the 'Manage Permissions' permission is set to Deny"
        };

        public Task<bool> EvaluateAsync(string projectId, string repositoryId, 
            IEnumerable<Response.MinimumNumberOfReviewersPolicy> policies)
        {
            if (projectId == null)
                throw new ArgumentNullException(nameof(projectId));
            if (repositoryId == null)
                throw new ArgumentNullException(nameof(repositoryId));

            return Permissions(projectId, repositoryId)
                .ValidateAsync();
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

        private ManagePermissions Permissions(string projectId, string repositoryId) =>
            ManagePermissions
                .ForRepository(_client, projectId, repositoryId)
                .Permissions(PermissionBitBypassPoliciesPullRequest, PermissionBitBypassPoliciesCodePush, 
                    PermissionBitManagePermissions)
                .Allow(PermissionId.NotSet, PermissionId.Deny, PermissionId.DenyInherited)
                .Ignore("Project Collection Administrators", "Project Collection Service Accounts");
    }
}