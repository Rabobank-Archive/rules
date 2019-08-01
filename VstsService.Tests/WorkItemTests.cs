using SecurePipelineScan.VstsService.Requests;
using SecurePipelineScan.VstsService.Response;
using Shouldly;
using System;
using System.Linq;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace SecurePipelineScan.VstsService.Tests
{
    public class WorkItemTests : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;
        private readonly IVstsRestClient _client;

        public WorkItemTests(TestConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _client = new VstsRestClient(config.Organization, config.Token);
        }

        [Fact]
        public async Task GetSingle_ShouldReturnWorkItem()
        {
            // Arrange
            var queryResult = await _client.PostAsync(WorkItemTracking.QueryByWiql(_config.Project, 1),
                new QueryByWiql($"select [System.Id] from WorkItems"));
            var workItem = queryResult.WorkItems.First();
            var fields = new[] {FieldNames.TeamProject};

            // Act
            var result =
                await _client.GetAsync(WorkItemTracking.GetWorkItem(_config.Project, workItem.Id, fields,
                    queryResult.AsOf));

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(workItem.Id);
            result.Fields.ShouldNotBeNull();
            result.Fields.ShouldContainKey(FieldNames.TeamProject);
            result.Fields[FieldNames.TeamProject].ShouldNotBeNull();
        }
    }
}