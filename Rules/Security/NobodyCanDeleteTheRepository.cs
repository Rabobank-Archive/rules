using System.Collections.Generic;
using System.Linq;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Requests;
using SecurePipelineScan.VstsService.Response;
using ApplicationGroup = SecurePipelineScan.VstsService.Response.ApplicationGroup;

namespace SecurePipelineScan.Rules.Security
{
    public class NobodyCanDeleteTheRepository : IRepositoryRule
    {
        private const string DeleteRepository = "Delete repository";
        private readonly IVstsRestClient _client;
        private const string DeleteTeamProject = "Delete team project";


        public NobodyCanDeleteTheRepository(IVstsRestClient client)
        {
            _client = client;
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
                groups.SelectMany(g => _client.Get(VstsService.Requests.Permissions.PermissionsGroupRepository(
                    projectId, namespaceGit, g.TeamFoundationId, repositoryId)).Permissions);

            return !permissions.Any(p => p.DisplayName == DeleteRepository &&
                                         (p.PermissionId == PermissionId.Allow ||
                                          p.PermissionId == PermissionId.AllowInherited));
        }

        public void Reconcile(string project, string repositoryId)
        {
            var projectId =
                _client.Get(VstsService.Requests.Project.Properties(project)).Id;

            var namespaceGit =
                _client.Get(VstsService.Requests.SecurityNamespace.SecurityNamespaces())
                    .First(s => s.DisplayName == "Git Repositories").NamespaceId;

            var groups = _client.Get(VstsService.Requests.ApplicationGroup.ExplicitIdentities(projectId, namespaceGit));

            foreach (var group in groups.Identities)
            {
                var permissions = _client.Get(VstsService.Requests.Permissions.PermissionsGroupRepository(
                        projectId, namespaceGit, group.TeamFoundationId, repositoryId));

                var permission = permissions.Permissions.Single(p => p.DisplayName == DeleteRepository);
                permission.PermissionId = 2;
                _client
                    .Post(Permissions.ManagePermissions(project,
                        new Permissions.ManagePermissionsData(
                            group.TeamFoundationId,
                            permissions.DescriptorIdentifier,
                            permissions.DescriptorIdentityType,
                            permission)));
            }
        }
    }
}