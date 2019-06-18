using System.Collections.Generic;
using System.Threading.Tasks;
using SecurePipelineScan.VstsService.Response;
using Task = System.Threading.Tasks.Task;

namespace SecurePipelineScan.VstsService
{
    public interface IVstsRestClient
    {
        Task<TResponse> GetAsync<TResponse>(IVstsRequest<TResponse> request) where TResponse : new();
        Task<TResponse> GetAsync<TResponse>(string url) where TResponse : new();
        IEnumerable<TResponse> Get<TResponse>(IVstsRequest<Multiple<TResponse>> request) where TResponse: new();
        Task<TResponse> PostAsync<TInput, TResponse>(IVstsRequest<TInput, TResponse> request, TInput body) where TResponse: new();
        Task<TResponse> PutAsync<TInput, TResponse>(IVstsRequest<TInput, TResponse> request, TInput body) where TResponse : new();
        Task DeleteAsync(IVstsRequest request);
    }
}