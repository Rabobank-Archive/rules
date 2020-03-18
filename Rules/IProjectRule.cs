using System.Threading.Tasks;

namespace AzureDevOps.Compliance.Rules
{
    public interface IProjectRule : IRule
    {
        Task<bool> EvaluateAsync(string projectId);
    }
}