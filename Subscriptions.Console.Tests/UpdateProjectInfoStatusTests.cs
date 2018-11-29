using System.Collections.Generic;
using ExpectedObjects;
using System.Linq;
using NSubstitute;
using SecurePipelineScan.VstsService;
using Xunit;
using Response = SecurePipelineScan.VstsService.Response;
using Requests = SecurePipelineScan.VstsService.Requests;

namespace Subscriptions.Console.Tests
{
    public class UpdateProjectInfoStatusTests
    {
        [Fact]
        public void UpdateProjectInfoStatusTests_Create()
        {
            // Arrange
            var subscriptions = new[]
            {
                new Response.Hook
                {
                    ConsumerId = "azureStorageQueue",
                    EventType = "ms.vss-release.deployment-completed-event",
                    PublisherInputs = new Response.PublisherInputs
                    {
                        ProjectId = "1"
                    }
                }
            };

            var projects = new[]
            {
                new Response.Project
                {
                    Id = "1"
                },
                new Response.Project
                {
                    Id = "2"
                }
            };


            // Act
            var projectInfos = Program.SubscriptionsPerProject(subscriptions, projects).ToList();

            // Assert
            new[]
            {
                new ProjectInfo
                {
                    Id = "1",
                    ReleaseDeploymentCompleted = true
                },
                new ProjectInfo
                {
                    Id = "2"
                }
            }.ToExpectedObject().ShouldMatch(projectInfos);
        }

        [Theory]
        [InlineData(true, true, true, true, 0)]
        [InlineData(true, false, false, false, 3)]
        [InlineData(false, true, false, false, 3)]
        [InlineData(false, false, false, true, 3)]
        [InlineData(false, false, false, false, 4)]
        public void OnlySetsHookIfNotAlreadySet(bool a, bool b, bool c, bool d, int calls)
        {
            // Arrange
            var client = Substitute.For<IVstsRestClient>();
            var items = new[]
            {
                new ProjectInfo
                {
                    ReleaseDeploymentCompleted = a,
                    GitPushed = b, 
                    BuildComplete = c,
                    GitPullRequestCreated = d
                }
            };

            // Act
            Program.AddHooksToProjects("asdf", "asdf", client, items);

            // Assert
            client.Received(calls).Execute(Arg.Any<IVstsRestRequest<Response.Hook>>());
        }
    }
}