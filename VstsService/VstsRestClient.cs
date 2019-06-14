using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Flurl.Http.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using SecurePipelineScan.VstsService.Converters;
using SecurePipelineScan.VstsService.Requests;
using Response = SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.VstsService
{
    public class VstsRestClient : IVstsRestClient
    {
        private readonly string _organization;
        private readonly string _token;

        public VstsRestClient(string organization, string token)
        {
            _organization = organization;
            _token = token;
            
            FlurlHttp.Configure(settings => {
                var jsonSettings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    Converters = { new PolicyConverter() }
                };
                settings.JsonSerializer = new NewtonsoftJsonSerializer(jsonSettings);
                settings.HttpClientFactory = new HttpClientFactory();
            });
        }

        public async Task<TResponse> GetAsync<TResponse>(IVstsRequest<TResponse> request) where TResponse: new()
        {
            return await new Url(request.BaseUri(_organization))
                .AllowHttpStatus(HttpStatusCode.NotFound)
                .AppendPathSegment(request.Resource)
                .SetQueryParams(request.QueryParams)
                .WithBasicAuth(string.Empty, _token)
                .GetJsonAsync<TResponse>();
        }

        public async Task<TResponse> GetAsync<TResponse>(string url) where TResponse : new()
        {
            return await new Url(url)
                .AllowHttpStatus(HttpStatusCode.NotFound)
                .WithBasicAuth(string.Empty, _token)
                .GetJsonAsync<TResponse>();
        }

        
        public IEnumerable<TResponse> Get<TResponse>(IVstsRequest<Response.Multiple<TResponse>> request) where TResponse : new()
        {
            return new MultipleEnumerator<TResponse>(request, _organization, _token);
        }

        public async Task<TResponse> PostAsync<TInput, TResponse>(IVstsRequest<TInput, TResponse> request, TInput body) where TResponse : new()
        {
            return await new Url(request.BaseUri(_organization))
                .AppendPathSegment(request.Resource)
                .WithBasicAuth(string.Empty, _token)
                .SetQueryParams(request.QueryParams)
                .PostJsonAsync(body)
                .ReceiveJson<TResponse>();
        }

        public async Task<TResponse> PutAsync<TInput, TResponse>(IVstsRequest<TInput, TResponse> request, TInput body) where TResponse : new()
        {
            return await new Url(request.BaseUri(_organization))
                .AppendPathSegment(request.Resource)
                .WithBasicAuth(string.Empty, _token)
                .SetQueryParams(request.QueryParams)
                .PutJsonAsync(body)
                .ReceiveJson<TResponse>();
        }

        public async Task DeleteAsync(IVstsRequest request)
        {
            await new Url(request.BaseUri(_organization))
                .AppendPathSegment(request.Resource)
                .WithBasicAuth(string.Empty, _token)
                .SetQueryParams(request.QueryParams)
                .DeleteAsync();
        }
    }
}