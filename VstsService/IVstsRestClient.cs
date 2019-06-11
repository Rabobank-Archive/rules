using System.Collections.Generic;
using System.Threading.Tasks;

namespace SecurePipelineScan.VstsService
{
    public interface IVstsRestClient
    {
        TResponse Get<TResponse>(IVstsRequest<TResponse> request) where TResponse: new();
        Task<TResponse> GetAsync<TResponse>(IVstsRequest<TResponse> request) where TResponse : new();
        IEnumerable<TResponse> Get<TResponse>(IVstsRequest<Response.Multiple<TResponse>> request) where TResponse: new();
        Task<IEnumerable<TResponse>> GetAsync<TResponse>(IVstsRequest<Response.Multiple<TResponse>> request) where TResponse: new();
        TResponse Post<TInput, TResponse>(IVstsRequest<TInput, TResponse> request, TInput body) where TResponse: new();
        Task<TResponse> PostAsync<TInput, TResponse>(IVstsRequest<TInput, TResponse> request, TInput body) where TResponse: new();
        TResponse Put<TInput, TResponse>(IVstsRequest<TInput, TResponse> request, TInput body) where TResponse : new();
        Task<TResponse> PutAsync<TInput, TResponse>(IVstsRequest<TInput, TResponse> request, TInput body) where TResponse : new();
        void Delete(IVstsRequest request);
        Task DeleteAsync(IVstsRequest request);
    }
}