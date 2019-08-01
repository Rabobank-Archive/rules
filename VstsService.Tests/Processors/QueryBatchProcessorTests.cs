using AutoFixture;
using NSubstitute;
using SecurePipelineScan.VstsService.Processors;
using SecurePipelineScan.VstsService.Response;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Xunit;

namespace SecurePipelineScan.VstsService.Tests.Processors
{
    public class QueryBatchProcessorTests
    {
        private readonly IVstsRestClient _client = Substitute.For<IVstsRestClient>();
        private readonly Fixture _fixture = new Fixture();

        [Fact]
        public void QueryByWiql_WithoutWhereClause_ShouldBeRunInBatchesOfSpecifiedSize()
        {
            // Arrange
            var projectName = _fixture.Create<string>();
            var workItemReferences = new[] {new WorkItemReference {Id = 42}, new WorkItemReference {Id = 96}};
            var processor = new QueryBatchProcessor(_client);
            
            MockQueryResultsByOne(workItemReferences);

            // Act
            var results = processor.QueryByWiql(projectName, batchSize: 1).ToImmutableList();

            // Assert
            results.ShouldBe(workItemReferences);
            _client
                .Received(workItemReferences.Length + 1)
                .PostAsync(
                    Arg.Is<IVstsRequest<QueryByWiql, WorkItemQueryResult>>(r =>
                        r.QueryParams.Any(p => p.Key == "$top" && 1.Equals(p.Value))),
                    Arg.Any<QueryByWiql>());
            AssertQueriesReceivedContain(workItemReferences, "SELECT [System.Id]");
        }

        [Fact]
        public void QueryByWiql_WithWhereClause_ShouldBeRunInBatchesOfSpecifiedSize()
        {
            // Arrange
            const string whereClause = "[System.ChangedDate] < @startOfDay";

            var projectName = _fixture.Create<string>();
            var workItemReferences = _fixture.CreateMany<WorkItemReference>().ToImmutableList();
            var processor = new QueryBatchProcessor(_client);

            MockQueryResultsByOne(workItemReferences);

            // Act
            var results = processor.QueryByWiql(projectName, whereClause, 1).ToImmutableList();

            // Assert
            results.ShouldBe(workItemReferences);
            AssertQueriesReceivedContain(workItemReferences, whereClause);
        }

        private void MockQueryResultsByOne(IEnumerable<WorkItemReference> workItemReferences)
        {
            var results = workItemReferences
                .Select(r => CreateWorkItemQueryResult(r))
                .Append(CreateWorkItemQueryResult())
                .ToImmutableList();
            
            _client
                .PostAsync(Arg.Any<IVstsRequest<QueryByWiql, WorkItemQueryResult>>(), Arg.Any<QueryByWiql>())
                .Returns(results.First(), results.Skip(1).ToArray());
        }

        private WorkItemQueryResult CreateWorkItemQueryResult(params WorkItemReference[] workItems) =>
            new WorkItemQueryResult
            {
                WorkItems = workItems.ToImmutableList(),
                AsOf = _fixture.Create<DateTime>()
            };

        private void AssertQueriesReceivedContain(ICollection<WorkItemReference> workItemReferences, string query)
        {
            _client
                .Received(workItemReferences.Count + 1)
                .PostAsync(
                    Arg.Any<IVstsRequest<QueryByWiql, WorkItemQueryResult>>(),
                    Arg.Is<QueryByWiql>(q => QueryContains(q, query)));
        }

        private static bool QueryContains(QueryByWiql queryByWiql, string expectedPart) =>
            queryByWiql.Query.Contains(expectedPart, StringComparison.InvariantCultureIgnoreCase);
    }
}