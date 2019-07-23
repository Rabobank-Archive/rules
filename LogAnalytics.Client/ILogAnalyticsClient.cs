using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace LogAnalytics.Client
{
    public interface ILogAnalyticsClient
    {
        Task AddCustomLogJsonAsync(string logName, object input, string timefield);
        Task<JObject> QueryAsync(string query);
    }
}