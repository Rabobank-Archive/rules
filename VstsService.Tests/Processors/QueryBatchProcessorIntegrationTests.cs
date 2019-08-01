using SecurePipelineScan.VstsService.Processors;
using Shouldly;
using System.Linq;
using Xunit;

namespace SecurePipelineScan.VstsService.Tests.Processors
{
    /// <summary>
    /// Tests work item retrieval in batches.
    /// </summary>
    /// <remarks>
    /// As we don't have enough work items in our default organisation (somecompany-test), we run this on  somecompanywith the
    /// project hard coded. When running this test, make sure your token in appsettings.user.json or an environment
    /// variable is valid for somecompany.
    /// </remarks>
    /// <see cref="TestConfig"/>
    public class QueryBatchProcessorIntegrationTests : IClassFixture<TestConfig>
    {
        private const string Project = "Investments";

        private readonly IQueryBatchProcessor _processor;

        public QueryBatchProcessorIntegrationTests(TestConfig config)
        {
            var client = new VstsRestClient("somecompany", config.Token);
            _processor = new QueryBatchProcessor(client);
        }

        [Fact(Skip = "integration")]
        public void ShouldBeAbleToReceiveMoreThan20KWorkItems()
        {
            // Arrange

            // Act
            var resultCount = _processor.QueryByWiql(Project).Count();

            // Assert
            resultCount.ShouldBeGreaterThan(20_000);
        }
    }
}