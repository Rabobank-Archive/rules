using System.Threading.Tasks;

namespace SecurePipelineScan.Rules.Security
{
    public interface IReconcile
    {
        Task ReconcileAsync(string projectId, string itemId, string scope, string stageId);

        string[] Impact { get; }
    }
}