namespace SecurePipelineScan.Rules.Security
{
    public interface IRule
    {
        string Description { get; }
        string Link { get; }
        bool IsSox { get; }
    }
}