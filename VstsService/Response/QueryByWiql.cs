using System;

namespace SecurePipelineScan.VstsService.Response
{
    public class QueryByWiql
    {
        public string Query { get; }

        public QueryByWiql(string query)
        {
            Query = query ?? throw new ArgumentNullException(nameof(query));
        }
    }
}