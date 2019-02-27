using Rules.Tests.Helpers;
using Shouldly;
using System;
using System.Linq;
using Xunit;
using Response = SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.Rules.Tests
{
    public class ReleaseBuilderTests
    {
        [Fact]
        public void ReleaseBuilder_Create_ShouldNotBeNull()
        {
            Response.Release release = ReleaseBuilder.Create();
            release.ShouldNotBeNull();
        }

        [Fact]
        public void ReleaseBuilder_Create_ShouldReturnRelease()
        {
            Guid approvedById = Guid.NewGuid();
            Guid modifiedById = Guid.NewGuid();
            Response.Release release = ReleaseBuilder.Create(approvedById, modifiedById);

            release.Environments.ShouldNotBeEmpty();
            release.Environments.First().DeploySteps.ShouldNotBeEmpty();
            release.Environments.First().DeploySteps.First().LastModifiedBy.Id.ShouldBe(modifiedById);

            release.Environments.First().PreDeployApprovals.ShouldNotBeEmpty();
            release.Environments.First().PreDeployApprovals.First().ApprovedBy.Id.ShouldBe(approvedById);
        }

        [Fact]
        public void ReleaseBuilder_WithPreDeployApprovalStatus_ShouldHaveStatus()
        {
            Response.Release release = ReleaseBuilder.Create().
                WithPreDeployApprovalStatus("Testingg");

            release.Environments.First().PreDeployApprovals.First().Status.ShouldBe("Testingg");
        }
    }
}