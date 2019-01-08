using Newtonsoft.Json.Linq;
using SecurePipelineScan.Rules.Reports;

namespace SecurePipelineScan.Rules.Events
{
    public interface IServiceHookScan<out TReport>
    {
        TReport Completed(JObject input);
    }
}