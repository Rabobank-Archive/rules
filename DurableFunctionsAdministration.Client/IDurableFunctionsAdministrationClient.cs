using System.Collections.Generic;
using System.Threading.Tasks;
using DurableFunctionsAdministration.Client.Request;

namespace DurableFunctionsAdministration.Client
{
    public interface IDurableFunctionsAdministrationClient
    {
        IEnumerable<TResponse> Get<TResponse>(IRestRequest<IEnumerable<TResponse>> request) where TResponse: new();
        Task<TResponse> GetAsync<TResponse>(IRestRequest<TResponse> request) where TResponse : new();
        Task DeleteAsync(IRestRequest request);
    }
}