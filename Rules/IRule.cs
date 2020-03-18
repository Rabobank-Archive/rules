namespace AzureDevOps.Compliance.Rules
{
    public interface IRule
    {
        string Description { get; }
        string Link { get; }
    }
}