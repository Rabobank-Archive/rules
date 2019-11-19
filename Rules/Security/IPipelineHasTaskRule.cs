namespace SecurePipelineScan.Rules.Security
{
    public interface IPipelineHasTaskRule
    {
        string TaskId { get; }
        string TaskName { get; }
        string StepName { get; }
    }
}