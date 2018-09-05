using RestSharp;

namespace vsts
{
    public interface IVstsRestClient
    {
        IRestResponse<TResponse> Execute<TResponse>(IVstsRestRequest<TResponse> request) where TResponse: new();
    }
}