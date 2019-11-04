using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SecurePipelineScan.VstsService;
using Response = SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.Rules.Security
{
    public class NobodyCanDeleteTheRepository : ItemHasPermissionRuleBase, IRepositoryRule, IReconcile
    {
        public NobodyCanDeleteTheRepository(IVstsRestClient client) : base(client)
        {
            //nothing
        }

        const int PermissionBitDeletRepository = 512;
        const int PermissionBitManagePermissions = 8192;

        protected override string NamespaceId => "2e9eb7ed-3c0a-47d4-87c1-0ffdd275fd87"; //Git Repositories
        protected override IEnumerable<int> PermissionBits => new[]
        {
            PermissionBitDeletRepository,
            PermissionBitManagePermissions
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
            "Project Collection Service Accounts"
        };

        public string Description => "Nobody can delete the repository";
        public string Link => "https://confluence.dev.somecompany.nl/x/RI8AD";
        public bool IsSox => true;

        string[] IReconcile.Impact => new[]
        {
            "For all security groups the 'Delete Repository' permission is set to Deny",
            "For all security groups the 'Manage Permissions' permission is set to Deny"
        };

        public async Task<bool> EvaluateAsync(string projectId, string repositoryId,
            IEnumerable<Response.MinimumNumberOfReviewersPolicy> policies)
        {
            if (projectId == null)
                throw new ArgumentNullException(nameof(projectId));
            if (repositoryId == null)
                throw new ArgumentNullException(nameof(repositoryId));

            return await base.EvaluateAsync(projectId, repositoryId, RuleScopes.Repositories)
                .ConfigureAwait(false);
        }

        public async Task ReconcileAsync(string projectId, string itemId, string scope, string stageId)
        {
            if (projectId == null)
                throw new ArgumentNullException(nameof(projectId));
            if (itemId == null)
                throw new ArgumentNullException(nameof(itemId));

            await ReconcileAsync(projectId, itemId, scope)
                .ConfigureAwait(false);
        }
    }
}