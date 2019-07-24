using System.Collections.Generic;

namespace DurableFunctionsAdministration.Client.Request
{
    public class RestRequest<TInput, TResponse> : IRestRequest<TInput, TResponse>
        
    {
        public string Resource { get; }
        public IDictionary<string, object> QueryParams { get; }

        public RestRequest(string resource) : this(resource, new Dictionary<string, object>())
        {
        }

        public RestRequest(string resource, IDictionary<string, object> queryParams)
        {
            Resource = resource;
            QueryParams = queryParams;
        }
    }

    public class RestRequest<TResponse> : RestRequest<TResponse, TResponse>, IRestRequest<TResponse>
    {
        public RestRequest(string resource) : base(resource)
        {
        }

        public RestRequest(string resource, IDictionary<string, object> queryParams) : base(resource, queryParams)
        {
        }
    }
}