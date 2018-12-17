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
    public class Permissions : IClassFixture<TestConfig>
    {
        private readonly TestConfig config;
        private readonly IVstsRestClient Vsts;

        public Permissions(TestConfig config)
        {
            this.config = config;
            Vsts = new VstsRestClient(config.Organization, config.Token);
        }

        [Fact]
        public void QueryPermissionsGroupRepositorySetReturnsPermissions()
        {
            var permissionsGitRepositorySet = Vsts.Get(Requests.PermissionsGroupRepoSet.PermissionsGitRepositorySet(config.Project));

            permissionsGitRepositorySet.ShouldNotBeNull();

            var firstPermission = permissionsGitRepositorySet.Permissions.First();
            firstPermission.PermissionBit.ShouldNotBeNull();
        }
    }
}