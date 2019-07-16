using System;
using System.Collections.Generic;

namespace DurableFunctionsAdministration.Client.Request
{
        public interface IRestRequest<TInput, TResponse> : IRestRequest
            where TResponse: new()
        {
        }

        public interface IRestRequest
        {
            string Resource { get; }
            IDictionary<string, object> QueryParams { get; }
        }

        public interface IRestRequest<TResponse> : IRestRequest<TResponse, TResponse>
            where TResponse : new()
        {
        }
}