using RestSharp;

namespace lib.Vsts
{
    public interface IVstsClient
    {
        IRestResponse<TResponse> Execute<TResponse>(IVstsRequest<TResponse> request) where TResponse: new();
    }
}