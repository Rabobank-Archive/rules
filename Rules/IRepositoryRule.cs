using System.Threading.Tasks;

namespace AzureDevOps.Compliance.Rules
{
    public interface IRepositoryRule : IRule
    {
        Task<bool?> EvaluateAsync(string projectId, string repositoryId);
    }
}