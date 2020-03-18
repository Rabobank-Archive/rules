using System.Collections.Generic;

namespace AzureDevOps.Compliancy.Rules
{
    public interface IPipelineHasTaskRule
    {
        string TaskId { get; }
        string TaskName { get; }
        Dictionary<string, string> Inputs { get; }
    }
}