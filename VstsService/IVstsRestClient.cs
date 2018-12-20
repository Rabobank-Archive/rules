using Newtonsoft.Json.Linq;

namespace SecurePipelineScan.VstsService
{
    public interface IVstsRestClient
    {
        TResponse Get<TResponse>(IVstsRestRequest<TResponse> request) where TResponse: new();
        TResponse Post<TResponse>(IVstsPostRequest<TResponse> request) where TResponse: new();
        void Delete(IVstsRestRequest subscription);
    }
}