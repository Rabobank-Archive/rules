using System.Threading.Tasks;

namespace SecurePipelineScan.VstsService
{
    public static class VstsRestClientExtensions
    {
        public static Task<TResponse> GetAsync<TResponse>(this IVstsRestClient client, string url) where TResponse: new()
        {
            return client.GetAsync(new VstsRequest<TResponse>(url));
        }
    }
}