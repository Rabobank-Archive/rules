using System.Collections.Generic;
using System.Threading.Tasks;

namespace SecurePipelineScan.VstsService
{
    public interface IVstsRestClient
    {
        Task<TResponse> GetAsync<TResponse>(IVstsRequest<TResponse> request) where TResponse : new();
        Task<IEnumerable<TResponse>> GetAsync<TResponse>(IVstsRequest<Response.Multiple<TResponse>> request) where TResponse: new();
        Task<TResponse> PostAsync<TInput, TResponse>(IVstsRequest<TInput, TResponse> request, TInput body) where TResponse: new();
        Task<TResponse> PutAsync<TInput, TResponse>(IVstsRequest<TInput, TResponse> request, TInput body) where TResponse : new();
        Task DeleteAsync(IVstsRequest request);
    }
}