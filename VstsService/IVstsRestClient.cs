using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace SecurePipelineScan.VstsService
{
    public interface IVstsRestClient
    {
        Task<TResponse> GetAsync<TResponse>(IVstsRequest<TResponse> request) where TResponse : new();
        Task<TResponse> GetAsync<TResponse>(Uri url) where TResponse : new();
        IEnumerable<TResponse> Get<TResponse>(IEnumerableRequest<TResponse> request);
        Task<TResponse> PostAsync<TInput, TResponse>(IVstsRequest<TInput, TResponse> request, TInput body) where TResponse: new();

        Task<TResponse> PatchAsync<TInput, TResponse>(IVstsRequest<TInput, TResponse> request, TInput body) where TResponse : new();
        Task<TResponse> PutAsync<TInput, TResponse>(IVstsRequest<TInput, TResponse> request, TInput body) where TResponse : new();
        Task DeleteAsync(IVstsRequest request);
    }
}