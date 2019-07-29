using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DurableFunctionsAdministration.Client.Request;
using Flurl;
using Flurl.Http;
using Flurl.Http.Configuration;
using Newtonsoft.Json;
using NullValueHandling = Newtonsoft.Json.NullValueHandling;

namespace DurableFunctionsAdministration.Client
{
    public class DurableFunctionsAdministrationClient : IDurableFunctionsAdministrationClient
    {
        private Uri BaseUri { get; set; }
        private string TaskHub { get; set; }
        private string Code { get; set; }
        public DurableFunctionsAdministrationClient(Uri baseUri, string taskHub, string code)
        {
            BaseUri = baseUri;
            TaskHub = taskHub;
            Code = code;
            
            FlurlHttp.Configure(settings => {
                settings.HttpClientFactory = new HttpClientFactory();
                var jsonSettings = new JsonSerializerSettings
                {
                    NullValueHandling =  NullValueHandling.Ignore
                };
                settings.JsonSerializer = new NewtonsoftJsonSerializer(jsonSettings);
            });
        }
        
        public Task<TResponse> GetAsync<TResponse>(IRestRequest<TResponse> request) where TResponse : new()
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return GetInternalAsync(request);
        }

        private async Task<TResponse> GetInternalAsync<TResponse>(IRestRequest<TResponse> request) where TResponse : new()
        {
            return await new Url(BaseUri)
                            .AppendPathSegment(request.Resource)
                            .SetQueryParams(request.QueryParams)
                            .SetQueryParam("taskHub", TaskHub)
                            .SetQueryParam("code", Code)
                            .GetJsonAsync<TResponse>()
                            .ConfigureAwait(false);
        }

        public IEnumerable<TResponse> Get<TResponse>(IRestRequest<IEnumerable<TResponse>> request) where TResponse : new()
        {
            return new MultipleEnumerator<TResponse>(request, BaseUri, TaskHub, Code);
        }
    }
}