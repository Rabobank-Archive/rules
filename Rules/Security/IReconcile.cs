using System.Threading.Tasks;

namespace SecurePipelineScan.Rules.Security
{
    public interface IReconcile
    {
        bool RequiresStageId { get; }

        Task ReconcileAsync(string projectId, string itemId, string stageId);

        string[] Impact { get; }
    }
}