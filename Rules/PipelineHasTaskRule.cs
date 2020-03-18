using System.Collections.Generic;

namespace AzureDevOps.Compliancy.Rules
{
    public class PipelineHasTaskRule : IPipelineHasTaskRule
    {
        public string TaskId { get; set; }
        public string TaskName { get; set; }
        public Dictionary<string, string> Inputs { get; set; }

        public PipelineHasTaskRule(string taskId)
        {
            TaskId = taskId;
        }
    }
}
