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
        private readonly TestConfig config;
        private readonly IVstsRestClient Vsts;

        public RepositoryPermissions(TestConfig config)
        {
            this.config = config;
            Vsts = new VstsRestClient(config.Organization, config.Token);
        }

        [Fact]
        public void QueryPermissionsGroupRepositorySetReturnsPermissions()
        {
    
            var nameSpaces = Vsts.Get(Requests.NameSpace.NameSpaces()).Value;

            var queryNameSpaceId =
                from ids in nameSpaces
                where ids.DisplayName == "Git Repositories"
                select ids.namespaceId;
            
            var groupIdentities = Vsts.Get(Requests.ApplicationGroup.ApplicationGroups(config.Project)).Identities;
            
            var queryApplicationGroupId = 
                from ids in groupIdentities
                where ids.DisplayName == $"[{config.Project}]\\Project Administrators"
                select ids.TeamFoundationId;
         
            var projectId = Vsts.Get(Requests.Project.ProjectProperties(config.Project)).Id;
            
            
            var permissionsGitRepositorySet = Vsts.Get(Requests.PermissionsGroupRepoSet.PermissionsGitRepositorySet(
                projectId, queryNameSpaceId.First(), queryApplicationGroupId.First()));

            permissionsGitRepositorySet.ShouldNotBeNull();
            var firstPermission = permissionsGitRepositorySet.Permissions.First();
            firstPermission.PermissionBit.ShouldNotBeNull();
        }
        
    }
}