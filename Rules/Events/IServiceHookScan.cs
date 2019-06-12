using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SecurePipelineScan.Rules.Reports;

namespace SecurePipelineScan.Rules.Events
{
    public interface IServiceHookScan<TReport>
    {
        Task<TReport> Completed(JObject input);
    }
}