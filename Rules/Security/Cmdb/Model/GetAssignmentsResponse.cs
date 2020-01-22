using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SecurePipelineScan.Rules.Security.Cmdb.Model
{
    [ExcludeFromCodeCoverage]
    public class GetAssignmentsResponse
    {
        public IEnumerable<AssignmentContentItem> Content { get; set; }
    }
}
