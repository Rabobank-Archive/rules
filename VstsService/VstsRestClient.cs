using RestSharp;

namespace SecurePipelineScan.VstsService
{
    public class VstsRestClient : IVstsRestClient
    {
        private readonly string authorization;
        private readonly string organization;
        private RestClient restClient;

        public VstsRestClient(string organization, string token)
        {
            this.authorization = GenerateAuthorizationHeader(token);
            this.organization = organization;
            restClient = new RestClient();
        }

        public IRestResponse<TResponse> Execute<TResponse>(IVstsRestRequest<TResponse> request)
            where TResponse : new()
        {
            restClient.BaseUrl = request.BaseUri(organization);

            return restClient.Execute<TResponse>(request.AddHeader("authorization", authorization));
        }

        private static string GenerateAuthorizationHeader(string token)
        {
            string encoded = Base64Encode($":{token}");
            return ($"Basic {encoded}");
        }

        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}