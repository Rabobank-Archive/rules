using System.Threading.Tasks;

namespace LogAnalytics.Client
{
    public interface IAzureTokenProvider
    {
        Task<string> GetAccessToken();
    }
}