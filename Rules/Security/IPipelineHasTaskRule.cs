using System.Collections.Generic;

namespace SecurePipelineScan.Rules.Security
{
    public interface IPipelineHasTaskRule
    {
        string TaskId { get; }
        string TaskName { get; }
        Dictionary<string, string> Inputs { get; }
    }
}