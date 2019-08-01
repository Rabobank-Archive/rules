using SecurePipelineScan.VstsService.Requests;
using SecurePipelineScan.VstsService.Response;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecurePipelineScan.VstsService.Processors
{
    public class QueryBatchProcessor : IQueryBatchProcessor
    {
        private const int DefaultBatchSize = 20_000 - 1;
        
        private readonly IVstsRestClient _client;

        public QueryBatchProcessor(IVstsRestClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public IEnumerable<WorkItemReference> QueryByWiql(string project, string whereClause = null,
            int batchSize = DefaultBatchSize) =>
                new BatchedQueryByWiql(_client, project, whereClause, batchSize);

        private class BatchedQueryByWiql : IEnumerable<WorkItemReference>
        {
            private readonly IVstsRestClient _client;
            private readonly string _project;
            private readonly int _batchSize;
            private readonly string _extraWhereClause;

            public BatchedQueryByWiql(IVstsRestClient client, string project, string whereClause, int batchSize)
            {
                _client = client ?? throw new ArgumentNullException(nameof(client));
                _project = project ?? throw new ArgumentNullException(nameof(project));
                _batchSize = batchSize;

                _extraWhereClause = string.IsNullOrEmpty(whereClause) ? string.Empty : $"AND ({whereClause})";
            }

            public IEnumerator<WorkItemReference> GetEnumerator()
            {
                var id = 0;

                while (true)
                {
                    var result = QuerySingleBatchAsync(id)
                        .ConfigureAwait(false)
                        .GetAwaiter().GetResult();

                    if (!result.WorkItems.Any())
                        break;
                    
                    foreach (var workItem in result.WorkItems)
                    {
                        yield return workItem;
                    }

                    id = result.WorkItems.Last().Id;
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            private Task<WorkItemQueryResult> QuerySingleBatchAsync(int id)
            {
                var fullWhereClause = $@"[{FieldNames.TeamProject}] = @Project AND [{FieldNames.Id}] > {
                        id
                    } {_extraWhereClause} ORDER BY [{FieldNames.Id}]";
                var query = $"SELECT [{FieldNames.Id}] FROM WorkItems WHERE {fullWhereClause}";

                return _client.PostAsync(WorkItemTracking.QueryByWiql(_project, _batchSize), new QueryByWiql(query));
            }
        }
    }
}