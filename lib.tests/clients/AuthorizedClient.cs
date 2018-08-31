using RestSharp;

namespace lib.tests.clients
{
    abstract class AuthorizedClient : RestClient
    {
        private readonly string token;

        public AuthorizedClient(string url, string token) : base(url)
        {
            this.token = token;
        }

        public override IRestResponse<T> Execute<T>(IRestRequest request)
        {
            return base.Execute<T>(request.AddHeader("authorization", GenerateAuthorizationHeader(token)));
        }

        private static string GenerateAuthorizationHeader(string token)
        {
            string encoded = Base64Encode($":{token}");
            return ($"Basic {encoded}");
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}

