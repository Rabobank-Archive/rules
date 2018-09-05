using RestSharp;

namespace SecurePipelineScan.VstsService
{
    public interface IVstsRestClient
    {
        IRestResponse<TResponse> Execute<TResponse>(IVstsRestRequest<TResponse> request) where TResponse: new();
    }
}