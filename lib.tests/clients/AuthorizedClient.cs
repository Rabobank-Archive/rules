using RestSharp;

namespace lib.tests.clients
{
    abstract class AuthorizedClient : RestClient
    {
        private readonly string authorization;

        public AuthorizedClient(string url, string token) : base(url)
        {
            this.authorization = GenerateAuthorizationHeader(token);
        }

        public override IRestResponse<T> Execute<T>(IRestRequest request)
        {
            return base.Execute<T>(request.AddHeader("authorization", authorization));
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

