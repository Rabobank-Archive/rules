using SecurePipelineScan.VstsService.Response;
using System.Collections.Generic;

namespace SecurePipelineScan.VstsService.Processors
{
    public interface IQueryBatchProcessor
    {
        IEnumerable<WorkItemReference> QueryByWiql(string project, string whereClause = null,
            int batchSize = 20_000 - 1);
    }
}