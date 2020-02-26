using System.Collections.Generic;

namespace SecurePipelineScan.Rules.Security
{
    public interface IPipelineHasTaskRule
    {
        string TaskId { get; }
        string TaskName { get; }
        string StepName { get; }
        Dictionary<string, object> Inputs { get; }
    }
}