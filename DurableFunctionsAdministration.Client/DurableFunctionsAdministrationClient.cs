using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DurableFunctionsAdministration.Client.Request;
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

            FlurlHttp.Configure(settings =>
            {
                settings.HttpClientFactory = new HttpClientFactory();
            });
        }

        public IEnumerable<TResponse> Get<TResponse>(IRestRequest<IEnumerable<TResponse>> request) where TResponse : new()
        {
            return new MultipleEnumerator<TResponse>(request, BaseUri, TaskHub, Code);
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

        public Task DeleteAsync(IRestRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return DeleteInternalAsync(request);
        }

        private async Task DeleteInternalAsync(IRestRequest request)
        {
            await new Url(BaseUri)
                .AppendPathSegment(request.Resource)
                .SetQueryParams(request.QueryParams)
                .SetQueryParam("taskHub", TaskHub)
                .SetQueryParam("code", Code)
                .DeleteAsync()
                .ConfigureAwait(false);
        }
    }
}