using System.Threading.Tasks;

namespace SecurePipelineScan.Rules.Security
{
    public interface IReconcile
    {
        Task Reconcile(string projectId, string id);
        
        string[] Impact { get; }
    }
}