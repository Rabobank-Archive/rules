using System.Collections.Generic;
using System.Linq;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.Rules.Security
{
    public class NobodyCanDeleteTheRepository : IRepositoryRule
    {
        private const string DeleteRepository = "Delete repository";
        private readonly IVstsRestClient _client;

        public NobodyCanDeleteTheRepository(IVstsRestClient client)
        {
            _client = client;
        }

        public string Description => "Nobody can delete the repository";

        public bool Evaluate(string project, string repository)
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

            return CheckNoBodyCanDeleteRepository(projectId, namespaceGit, repository, groups);
        }

        private bool CheckNoBodyCanDeleteRepository(string projectId, string namespaceGit, string repository, IEnumerable<ApplicationGroup> groups)
        {

            var permissions =
                groups.SelectMany(g => _client.Get(VstsService.Requests.Permissions.PermissionsGroupRepository(
                    projectId, namespaceGit, g.TeamFoundationId, repository)).Permissions);

            return !permissions.Any(p => p.DisplayName == DeleteRepository &&
                                         (p.PermissionId == PermissionId.Allow ||
                                          p.PermissionId == PermissionId.AllowInherited));
        }
    }
}