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
        private readonly string _authorization;
        private readonly string _organization;
        private readonly string _token;
        private readonly IRestClientFactory _factory;

        public VstsRestClient(string organization, string token, IRestClientFactory factory)
        {
            _authorization = GenerateAuthorizationHeader(token);
            _organization = organization;
            _token = token;
            _factory = factory;
            
            FlurlHttp.Configure(settings => {
                var jsonSettings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };
                settings.JsonSerializer = new NewtonsoftJsonSerializer(jsonSettings);
            });
        }

        internal VstsRestClient(string organization, string token) : this(organization, token, new RestClientFactory())
        {   
        }

        public async Task<TResponse> GetAsync<TResponse>(IVstsRequest<TResponse> request) where TResponse: new()
        {
            TResponse retval = default(TResponse);
            try
            {
                retval = await new Url(request.BaseUri(_organization))
                    .AppendPathSegment(request.Resource)
                    .SetQueryParams(request.QueryParams)
                    .WithBasicAuth(string.Empty, _token)
                    .GetJsonAsync<TResponse>();
            }
            catch (FlurlHttpException ex)
            {
                if (ex.Call.HttpStatus == HttpStatusCode.NotFound)
                {
                    retval = default(TResponse);
                }
                else
                {
                    throw;
                }
            }

            return retval;
        }
        
        public TResponse Get<TResponse>(IVstsRequest<TResponse> request)
            where TResponse : new()
        {
            return GetAsync(request).GetAwaiter().GetResult();
        }

        public IEnumerable<TResponse> Get<TResponse>(IVstsRequest<Response.Multiple<TResponse>> request) where TResponse : new()
        {
            return GetAsync(request).GetAwaiter().GetResult();
        }

        public async Task<IEnumerable<TResponse>> GetAsync<TResponse>(IVstsRequest<Response.Multiple<TResponse>> request) where TResponse : new()
        {
            return new MultipleEnumerator<TResponse>(request, _organization, _token);
                      
//
// var client = _factory.Create(request.BaseUri(_organization));
//            var wrapper = new RestRequest(request.Uri)
//                .AddHeader("authorization", _authorization);
//
//            return new MultipleEnumerator<TResponse>(wrapper, client);
        }


        public TResponse Post<TInput, TResponse>(IVstsRequest<TInput, TResponse> request, TInput body) where TResponse : new()
        {
            return PostAsync(request, body).GetAwaiter().GetResult();
        }

        public async Task<TResponse> PostAsync<TInput, TResponse>(IVstsRequest<TInput, TResponse> request, TInput body) where TResponse : new()
        {
            TResponse retval = default(TResponse);
            retval = await new Url(request.BaseUri(_organization))
                .AppendPathSegment(request.Resource)
                .WithBasicAuth(string.Empty, _token)
                .SetQueryParams(request.QueryParams)
                .PostJsonAsync(body)
                .ReceiveJson<TResponse>();

            return retval;
        }

        public TResponse Put<TInput, TResponse>(IVstsRequest<TInput, TResponse> request, TInput body) where TResponse: new()
        {
            return PutAsync(request, body).GetAwaiter().GetResult();
        }

        public async Task<TResponse> PutAsync<TInput, TResponse>(IVstsRequest<TInput, TResponse> request, TInput body) where TResponse : new()
        {
            TResponse retval = default(TResponse);
            retval = await new Url(request.BaseUri(_organization))
                .AppendPathSegment(request.Resource)
                .WithBasicAuth(string.Empty, _token)
                .SetQueryParams(request.QueryParams)
                .PutJsonAsync(body)
                .ReceiveJson<TResponse>();

            return retval;
        }

        public void Delete(IVstsRequest request)
        {
            DeleteAsync(request).GetAwaiter().GetResult();
        }

        public async Task DeleteAsync(IVstsRequest request)
        {
            await new Url(request.BaseUri(_organization))
                .AppendPathSegment(request.Resource)
                .WithBasicAuth(string.Empty, _token)
                .SetQueryParams(request.QueryParams)
                .DeleteAsync();
        }

        private static string GenerateAuthorizationHeader(string token)
        {
            var encoded = Base64Encode($":{token}");
            return ($"Basic {encoded}");
        }

        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}