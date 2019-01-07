using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Tests;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Xunit;

namespace SecurePipelineScan.VstsService.Tests
{
    [Trait("Category","integration")]
    public class RepositoryPermissions : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;
        private readonly IVstsRestClient _client;

        public RepositoryPermissions(TestConfig config)
        {
            this._config = config;
            _client = new VstsRestClient(config.Organization, config.Token);
        }

        [Fact]
        public void QueryPermissionsGroupRepositorySetReturnsPermissions()
        {
            var namespaceId = _client.Get(Requests.SecurityNamespace.SecurityNamespaces()).Value
                .First(ns => ns.DisplayName == "Git Repositories").NamespaceId;
                
            var applicationGroupId = _client.Get(Requests.ApplicationGroup.ApplicationGroups(_config.Project)).Identities
                .First(gi => gi.DisplayName == $"[{_config.Project}]\\Project Administrators").TeamFoundationId;
         
            var projectId = _client.Get(Requests.Project.Properties(_config.Project)).Id;
            
            
            var permissionsGitRepositorySet = _client.Get(Requests.Permissions.PermissionsGroupRepositorySet(
                projectId, namespaceId, applicationGroupId));

            permissionsGitRepositorySet.ShouldNotBeNull();
            permissionsGitRepositorySet.Permissions.First().ShouldNotBeNull();
        }
        
        [Fact]
        public void QueryPermissionsGroupRepositoryReturnsPermissions()
        {   
            var namespaceId = _client.Get(Requests.SecurityNamespace.SecurityNamespaces()).Value
                .First(ns => ns.DisplayName == "Git Repositories").NamespaceId;
                
            var applicationGroupId = _client.Get(Requests.ApplicationGroup.ApplicationGroups(_config.Project)).Identities
                .First(gi => gi.DisplayName == $"[{_config.Project}]\\Project Administrators").TeamFoundationId;
         
            var projectId = _client.Get(Requests.Project.Properties(_config.Project)).Id;

            
            var repositories = _client.Get(Requests.Repository.Repositories(_config.Project)).Value;

            foreach (var repository in repositories) 
            {
                var permissionsGitRepository = _client.Get(Requests.Permissions.PermissionsGroupRepository(
                    projectId, namespaceId, applicationGroupId, repository.Id));

                permissionsGitRepository.ShouldNotBeNull();
                permissionsGitRepository.Permissions.First().ShouldNotBeNull();
            }
        }
        
        [Fact]
        public void QueryPermissionsGroupSetIdReturnsPermissionsForSetId()
        {
            var permissionSetId = _client.Get(Requests.SecurityNamespace.SecurityNamespaces()).Value
                .First(ns => ns.Name == "Build").NamespaceId;
                
            var applicationGroupId = _client.Get(Requests.ApplicationGroup.ApplicationGroups(_config.Project)).Identities
                .First(gi => gi.DisplayName == $"[{_config.Project}]\\Project Administrators").TeamFoundationId;
         
            var projectId = _client.Get(Requests.Project.Properties(_config.Project)).Id;
            
            var permissionsGroupSetId = _client.Get(Requests.Permissions.PermissionsGroupSetId(
                projectId, permissionSetId, applicationGroupId));
            
            permissionsGroupSetId.ShouldNotBeNull();
            permissionsGroupSetId.CurrentTeamFoundationId.ShouldNotBeNull();
            permissionsGroupSetId.Permissions.First().ShouldNotBeNull();
        }

        [Fact]
        public void QueryPermissionsGroupSetIdDefinitionReturnsPermissionsForSetId()
        {
            var permissionSetId = _client.Get(Requests.SecurityNamespace.SecurityNamespaces()).Value
                .First(ns => ns.Name == "Build").NamespaceId;
                
            var applicationGroupId = _client.Get(Requests.ApplicationGroup.ApplicationGroups(_config.Project)).Identities
                .First(gi => gi.DisplayName == $"[{_config.Project}]\\Project Administrators").TeamFoundationId;
         
            var projectId = _client.Get(Requests.Project.Properties(_config.Project)).Id;

            var buildDefinitions = _client.Get(Requests.Builds.BuildDefinitions(projectId));

            foreach (var buildDefinition in buildDefinitions)
            {
                var permissionsGroupSetId = _client.Get(Requests.Permissions.PermissionsGroupSetIdDefinition(
                    projectId, permissionSetId, applicationGroupId, buildDefinition.id));
                
                permissionsGroupSetId.ShouldNotBeNull();
                permissionsGroupSetId.Permissions.First().PermissionId.ShouldNotBeNull();
            }
        }

        [Fact]
        public void QueryBuildDefinitionsReturnsBuildDefinitions()
        {
            var projectId = _client.Get(Requests.Project.Properties(_config.Project)).Id;

            var buildDefinitions = _client.Get(Requests.Builds.BuildDefinitions(projectId));
            
            buildDefinitions.ShouldNotBeNull();
            buildDefinitions.First().id.ShouldNotBeNull();
        }
    }
}