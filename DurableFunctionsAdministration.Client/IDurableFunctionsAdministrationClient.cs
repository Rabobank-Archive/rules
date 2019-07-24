using System.Collections.Generic;
using System.Threading.Tasks;
using DurableFunctionsAdministration.Client.Request;

namespace DurableFunctionsAdministration.Client
{
    public interface IDurableFunctionsAdministrationClient
    {
        Task<TResponse> GetAsync<TResponse>(IRestRequest<TResponse> request) where TResponse : new();
        IEnumerable<TResponse> Get<TResponse>(IRestRequest<IEnumerable<TResponse>> request) where TResponse: new();

    }
}