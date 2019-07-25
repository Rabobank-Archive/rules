using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace SecurePipelineScan.Rules.Events
{
    public interface IServiceHookScan<TReport>
    {
        Task<TReport> GetCompletedReportAsync(JObject input);
    }
}