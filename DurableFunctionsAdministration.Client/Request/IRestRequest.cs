using System.Collections.Generic;

namespace DurableFunctionsAdministration.Client.Request
{
        public interface IRestRequest<TInput, TResponse> : IRestRequest
        {
        }

        public interface IRestRequest
        {
            string Resource { get; }
            IDictionary<string, object> QueryParams { get; }
        }

        public interface IRestRequest<TResponse> : IRestRequest<TResponse, TResponse>
        {
        }
}