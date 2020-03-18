using System.Threading.Tasks;

namespace AzureDevOps.Compliance.Rules
{
    public interface IReconcile
    {
        Task ReconcileAsync(string projectId, string itemId);

        string[] Impact { get; }
    }
}