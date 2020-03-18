using System.Threading.Tasks;

namespace AzureDevOps.Compliance.Rules
{
    public interface IProjectReconcile
    {
        Task ReconcileAsync(string projectId);
        string[] Impact { get; }
    }
}