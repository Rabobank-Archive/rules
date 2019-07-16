using System.Threading.Tasks;
using DurableFunctionsAdministration.Client.Request;

namespace DurableFunctionsAdministration.Client
{
    public interface IDurableFunctionsAdministrationClient
    {
        Task<TResponse> GetAsync<TResponse>(IRestRequest<TResponse> request) where TResponse : new();
    }
}