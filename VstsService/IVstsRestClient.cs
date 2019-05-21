using Newtonsoft.Json.Linq;

namespace SecurePipelineScan.VstsService
{
    public interface IVstsRestClient
    {
        TResponse Get<TResponse>(IVstsRequest<TResponse> request) where TResponse: new();
        TResponse Post<TInput, TResponse>(IVstsRequest<TInput, TResponse> request, TInput body) where TResponse: new();
        TResponse Put<TInput, TResponse>(IVstsRequest<TInput, TResponse> request, TInput body) where TResponse : new();
        void Delete(IVstsRequest request);
    }
}