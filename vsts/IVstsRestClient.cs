using RestSharp;

namespace Vsts
{
    public interface IVstsRestClient
    {
        IRestResponse<TResponse> Execute<TResponse>(IVstsRestRequest<TResponse> request) where TResponse: new();
    }
}