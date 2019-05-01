using System.Collections.Generic;
using System.Linq;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Requests;
using SecurePipelineScan.VstsService.Response;
using ApplicationGroup = SecurePipelineScan.VstsService.Response.ApplicationGroup;

namespace SecurePipelineScan.Rules.Security
{
    public class NobodyCanDeleteTheRepository : IRepositoryRule, IRepositoryReconcile
    {
        private const string DeleteRepository = "Delete repository";
        private static readonly int[] AllowedPermissions = { PermissionId.NotSet, PermissionId.Deny, PermissionId.DenyInherited };
       
        private readonly IVstsRestClient _client;
        private readonly string _namespaceGit;

        public NobodyCanDeleteTheRepository(IVstsRestClient client)
        {
            _client = client;
            _namespaceGit = _client
                .Get(VstsService.Requests.SecurityNamespace.SecurityNamespaces())
                .First(s => s.DisplayName == "Git Repositories").NamespaceId;
        }

        string IRepositoryRule.Description => "Nobody can delete the repository";

        string IRepositoryRule.Why => "To enforce auditability, no data should be deleted. Therefore, nobody should be able to delete the repository.";

        public bool Evaluate(string project, string repositoryId)
        {
            var projectId =
                _client.Get(VstsService.Requests.Project.Properties(project)).Id;

            var namespaceGit =
                _client.Get(VstsService.Requests.SecurityNamespace.SecurityNamespaces())
                    .First(s => s.DisplayName == "Git Repositories").NamespaceId;

            var groups =
                _client.Get(VstsService.Requests.ApplicationGroup.ExplicitIdentities(projectId, namespaceGit))
                    .Identities
                    .Where(g => g.FriendlyDisplayName != "Project Collection Administrators" &&
                                g.FriendlyDisplayName != "Project Collection Service Accounts");

            return CheckNoBodyCanDeleteRepository(projectId, namespaceGit, repositoryId, groups);
        }

        private bool CheckNoBodyCanDeleteRepository(string projectId, string namespaceGit, string repositoryId,
            IEnumerable<ApplicationGroup> groups)
        {
            var permissions =
                groups.SelectMany(g => _client.Get(Permissions.PermissionsGroupRepository(
                    projectId, namespaceGit, g.TeamFoundationId, repositoryId)).Permissions);

            return permissions.All(p => p.DisplayName != DeleteRepository || AllowedPermissions.Contains(p.PermissionId));
        }

        public void Reconcile(string projectId, string repositoryId)
        {
            var groups = _client.Get(VstsService.Requests.ApplicationGroup.ExplicitIdentities(projectId, _namespaceGit));
            foreach (var group in groups.Identities)
            {
                UpdateDeleteRepositoryPermissionToDeny(projectId, repositoryId, _namespaceGit, group);
            }
        }

        private void UpdateDeleteRepositoryPermissionToDeny(string projectId,
            string repositoryId,
            string namespaceGit, ApplicationGroup @group)
        {
            var permissions = _client.Get(Permissions.PermissionsGroupRepository(
                projectId, namespaceGit, @group.TeamFoundationId, repositoryId));

            var permission = permissions.Permissions.Single(p => p.DisplayName == DeleteRepository);
            if (!AllowedPermissions.Contains(permission.PermissionId))
            {
                permission.PermissionId = PermissionId.Deny;
                _client
                    .Post(Permissions.ManagePermissions(projectId,
                        new Permissions.ManagePermissionsData(
                            @group.TeamFoundationId,
                            permissions.DescriptorIdentifier,
                            permissions.DescriptorIdentityType,
                            permission)));
            }
        }
    }
}