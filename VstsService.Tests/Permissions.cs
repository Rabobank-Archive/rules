using System.Linq;
using System.Threading.Tasks;
using SecurePipelineScan.VstsService.Requests;
using Shouldly;
using Xunit;

namespace SecurePipelineScan.VstsService.Tests
{
    [Trait("Category", "integration")]
    public class Permissions : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;
        private readonly IVstsRestClient _client;

        public Permissions(TestConfig config)
        {
            _config = config;
            _client = new VstsRestClient(config.Organization, config.Token);
        }

        [Fact]
        public async Task QueryPermissionsGroupRepositorySetReturnsPermissions()
        {
            var namespaceId = _client.Get(SecurityNamespace.SecurityNamespaces())
                .First(ns => ns.DisplayName == "Git Repositories").NamespaceId;

            var applicationGroupId = (await _client.GetAsync(Requests.ApplicationGroup.ApplicationGroups(_config.Project))).Identities
                .First(gi => gi.DisplayName == $"[{_config.Project}]\\Project Administrators").TeamFoundationId;

            var projectId = (await _client.GetAsync(Project.Properties(_config.Project))).Id;

            var permissionsGitRepositorySet = await _client.GetAsync(Requests.Permissions.PermissionsGroupRepositorySet(
                projectId, namespaceId, applicationGroupId));

            permissionsGitRepositorySet.ShouldNotBeNull();
            permissionsGitRepositorySet.Permissions.First().ShouldNotBeNull();
        }

        [Fact]
        public async Task QueryPermissionsGroupRepositoryReturnsPermissions()
        {
            var namespaceId = _client.Get(SecurityNamespace.SecurityNamespaces())
                .First(ns => ns.DisplayName == "Git Repositories").NamespaceId;

            var applicationGroupId = (await _client.GetAsync(Requests.ApplicationGroup.ApplicationGroups(_config.Project))).Identities
                .First(gi => gi.DisplayName == $"[{_config.Project}]\\Project Administrators").TeamFoundationId;

            var projectId = (await _client.GetAsync(Project.Properties(_config.Project))).Id;

            var repositories = _client.Get(Requests.Repository.Repositories(_config.Project));

            foreach (var repository in repositories)
            {
                var permissionsGitRepository = (await _client.GetAsync(Requests.Permissions.PermissionsGroupRepository(
                    projectId, namespaceId, applicationGroupId, repository.Id)));

                permissionsGitRepository.ShouldNotBeNull();
                permissionsGitRepository.Permissions.First().ShouldNotBeNull();
            }
        }

        [Fact]
        public async Task QueryPermissionsGroupBranchReturnsPermissions()
        {
            var namespaceId = _client.Get(SecurityNamespace.SecurityNamespaces())
                .First(ns => ns.DisplayName == "Git Repositories").NamespaceId;

            var applicationGroupId = (await _client.GetAsync(Requests.ApplicationGroup.ApplicationGroups(_config.Project))).Identities
                .First(gi => gi.DisplayName == $"[{_config.Project}]\\Project Administrators").TeamFoundationId;

            var projectId = (await _client.GetAsync(Project.Properties(_config.Project))).Id;

            var repositories = _client.Get(Requests.Repository.Repositories(_config.Project));

            foreach (var repository in repositories)
            {
                var permissionsMasterBranch = (await _client.GetAsync(Requests.Permissions.PermissionsGroupMasterBranch(
                    projectId, namespaceId, applicationGroupId, repository.Id)));

                permissionsMasterBranch.ShouldNotBeNull();
                permissionsMasterBranch.Permissions.First().ShouldNotBeNull();
            }
        }

        [Fact]
        public async Task QueryPermissionsGroupSetIdReturnsPermissionsForSetId()
        {
            var permissionSetId = _client.Get(SecurityNamespace.SecurityNamespaces())
                .First(ns => ns.Name == "Build").NamespaceId;

            var applicationGroupId = (await _client.GetAsync(Requests.ApplicationGroup.ApplicationGroups(_config.Project))).Identities
                .First(gi => gi.DisplayName == $"[{_config.Project}]\\Project Administrators").TeamFoundationId;

            var projectId = (await _client.GetAsync(Project.Properties(_config.Project))).Id;

            var permissionsGroupSetId = (await _client.GetAsync(Requests.Permissions.PermissionsGroupSetId(
                projectId, permissionSetId, applicationGroupId)));

            permissionsGroupSetId.ShouldNotBeNull();
            permissionsGroupSetId.CurrentTeamFoundationId.ShouldNotBeNull();
            permissionsGroupSetId.Permissions.First().ShouldNotBeNull();
        }

        [Fact]
        public async Task QueryPermissionsGroupSetIdDefinitionReturnsPermissionsForSetId()
        {
            var permissionSetId = _client.Get(SecurityNamespace.SecurityNamespaces())
                .First(ns => ns.Name == "Build").NamespaceId;

            var applicationGroupId = (await _client.GetAsync(Requests.ApplicationGroup.ApplicationGroups(_config.Project))).Identities
                .First(gi => gi.DisplayName == $"[{_config.Project}]\\Project Administrators").TeamFoundationId;

            var projectId = (await _client.GetAsync(Project.Properties(_config.Project))).Id;
            var buildDefinitions = _client.Get(Requests.Builds.BuildDefinitions(projectId));

            foreach (var buildDefinition in buildDefinitions)
            {
                var permissionsGroupSetId = (await _client.GetAsync(Requests.Permissions.PermissionsGroupSetIdDefinition(
                    projectId, permissionSetId, applicationGroupId, buildDefinition.Id)));

                permissionsGroupSetId.ShouldNotBeNull();
                permissionsGroupSetId.Permissions.First().PermissionId.ShouldNotBeNull();
            }
        }

        [Fact]
        public async Task SetPermissionsAsync()
        {
            await _client.PostAsync(Requests.Permissions.ManagePermissions(_config.Project),
                new Requests.ManagePermissionsData(
                    "2c12fa83-5bdb-4085-a635-c7cd00cdfba5",
                    "S-1-9-1551374245-50807123-3856808002-2418352955-3620213171-1-1337613045-2794958661-2397635820-2543327080",
                    "Microsoft.TeamFoundation.Identity", "vstfs:///Classification/TeamProject/53410703-e2e5-4238-9025-233bd7c811b3",
                    new Response.Permission
                    {
                        PermissionId = 2,
                        PermissionBit = 4,
                        NamespaceId = "52d39943-cb85-4d7f-8fa8-c6baac873819",
                        PermissionToken = "$PROJECT:vstfs:///Classification/TeamProject/53410703-e2e5-4238-9025-233bd7c811b3:"
                    }
                ).Wrap());
        }
    }
}