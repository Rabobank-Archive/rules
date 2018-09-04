using lib.Requests;
using RestSharp;

namespace lib
{
    public class VstsClient : IVstsClient
    {
        private readonly string authorization;
        private readonly string organization;

        public VstsClient(string organization, string token) : base()
        {
            this.authorization = GenerateAuthorizationHeader(token);
            this.organization = organization;
        }

        public IRestResponse<TResponse> Execute<TResponse>(IVstsRequest<TResponse> request)
            where TResponse: new()
        {
            var uri = new System.Uri($"https://{organization}.visualstudio.com/");
            if (request is IVsrmRequest<TResponse>)
            {
                uri = new System.Uri($"https://{organization}.vsrm.visualstudio.com/");
            }

            return new RestClient(uri).Execute<TResponse>(request.AddHeader("authorization", authorization));
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

