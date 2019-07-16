using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DurableFunctionsAdministration.Client.Request;
using DurableFunctionsAdministration.Client.Response;
using Flurl;
using Flurl.Http;

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
            });
        }
        
        public async Task<TResponse> GetAsync<TResponse>(IRestRequest<TResponse> request) where TResponse : new()
        {
            return await new Url(BaseUri)
                .AppendPathSegment(request.Resource)
                .SetQueryParams(request.QueryParams)
                .SetQueryParam("taskHub", TaskHub)
                .SetQueryParam("code", Code)
                .GetJsonAsync<TResponse>()
                .ConfigureAwait(false);
        }
    }
}