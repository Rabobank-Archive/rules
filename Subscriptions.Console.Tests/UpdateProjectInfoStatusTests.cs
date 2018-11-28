using ExpectedObjects;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Response = SecurePipelineScan.VstsService.Response;

namespace Subscriptions.Console.Tests
{
    public class UpdateProjectInfoStatusTests
    {
        [Fact]
        public void UpdateProjectInfoStatusTests_Create()
        {
            var subscriptions = new List<Response.Hook>()
            {
                new Response.Hook {
                    ConsumerId = "azureStorageQueue",
                    EventType = "ms.vss-release.deployment-completed-event",
                    PublisherInputs = new Response.PublisherInputs
                    {
                        ProjectId = "1"
                    }
                }
            };

            var projectInfos = new List<ProjectInfo>();

            projectInfos.AddRange(new[] {
                new ProjectInfo() {Id = "1" },
                new ProjectInfo() {Id = "2" },
            });

            Program.UpdateProjectInfoStatus(projectInfos, subscriptions);

            

            new ProjectInfo() { Id = "1", ReleaseDeploymentCompleted = true }.
                ToExpectedObject().
                ShouldMatch(projectInfos[0]);

            new ProjectInfo() { Id = "2"}.
                ToExpectedObject().
                ShouldMatch(projectInfos[1]);
        }
    }
}