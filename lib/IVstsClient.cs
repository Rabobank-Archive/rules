using RestSharp;

namespace lib
{
    public interface IVstsClient
    {
        IRestResponse<TResponse> Execute<TResponse>(IVstsRequest<TResponse> request) where TResponse: new();
    }
}