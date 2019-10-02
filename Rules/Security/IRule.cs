namespace SecurePipelineScan.Rules.Security
{
    public interface IRule
    {
        string Description { get; }
        string Why { get; }
        bool IsSox { get; }
    }
}