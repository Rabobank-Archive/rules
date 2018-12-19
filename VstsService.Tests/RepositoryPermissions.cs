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
    
            var namespaces = _client.Get(Requests.Namespace.SecurityNamespaces()).Value;

            var queryNamespaceId =
                from ns in namespaces
                where ns.DisplayName == "Git Repositories"
                select ns.NamespaceId;
            
            var groupIdentities = _client.Get(Requests.ApplicationGroup.ApplicationGroups(_config.Project)).Identities;
            
            var queryApplicationGroupId = 
                from gi in groupIdentities
                where gi.DisplayName == $"[{_config.Project}]\\Project Administrators"
                select gi.TeamFoundationId;
         
            var projectId = _client.Get(Requests.Project.ProjectProperties(_config.Project)).Id;
            
            
            var permissionsGitRepositorySet = _client.Get(Requests.PermissionsGroupRepoSet.PermissionsGitRepositorySet(
                projectId, queryNamespaceId.First(), queryApplicationGroupId.First()));

            permissionsGitRepositorySet.ShouldNotBeNull();
            var firstPermission = permissionsGitRepositorySet.Permissions.First();
            firstPermission.PermissionBit.ShouldNotBeNull();
        }
        
    }
}