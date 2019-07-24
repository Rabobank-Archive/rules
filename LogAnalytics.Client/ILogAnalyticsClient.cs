using System.Threading.Tasks;
using LogAnalytics.Client.Response;

namespace LogAnalytics.Client
{
    public interface ILogAnalyticsClient
    {
        Task AddCustomLogJsonAsync(string logName, object input, string timefield);
        Task<LogAnalyticsQueryResponse> QueryAsync(string query);
    }
}