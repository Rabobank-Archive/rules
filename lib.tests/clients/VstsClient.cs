using lib.tests.Requests;
using RestSharp;

namespace lib.tests.clients
{
    class VstsClient : RestClient
    {
        private readonly string authorization;
        private readonly string organization;

        private readonly object mutex = new object();

        public VstsClient(string organization, string token) : base()
        {
            this.authorization = GenerateAuthorizationHeader(token);
            this.organization = organization;
        }

        public override IRestResponse<T> Execute<T>(IRestRequest request)
        {
            lock (mutex)
            {
                switch (request)
                {
                    case IVsrmRequest r:
                        BaseUrl = new System.Uri($"https://{organization}.vsrm.visualstudio.com/");
                        break;
                    default:
                        BaseUrl = new System.Uri($"https://{organization}.visualstudio.com/");
                        break;
                }

                return base.Execute<T>(request.AddHeader("authorization", authorization));
            }
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

