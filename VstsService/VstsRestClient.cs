using RestSharp;

namespace SecurePipelineScan.VstsService
{
    public class VstsRestClient : IVstsRestClient
    {
        private readonly string authorization;
        private readonly string organization;

        public VstsRestClient(string organization, string token) : base()
        {
            this.authorization = GenerateAuthorizationHeader(token);
            this.organization = organization;
        }

        public IRestResponse<TResponse> Execute<TResponse>(IVstsRestRequest<TResponse> request)
            where TResponse: new()
        {
            return new RestClient(request.BaseUri(organization)).Execute<TResponse>(request.AddHeader("authorization", authorization));
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

