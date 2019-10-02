using Flurl.Http;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using LogAnalytics.Client.Response;

namespace LogAnalytics.Client
{
    /// <summary>
    /// Copied from: https://docs.microsoft.com/en-us/azure/azure-monitor/platform/data-collector-api#c-sample
    /// </summary>
    public class LogAnalyticsClient : ILogAnalyticsClient
    {
        private readonly string _workspace;
        private readonly string _key;
        private readonly IAzureTokenProvider _tokenprovider;

        public LogAnalyticsClient(string workspace, string key, IAzureTokenProvider tokenProvider)
        {
            _workspace = workspace ?? throw new ArgumentNullException(nameof(workspace));
            _key = key ?? throw new ArgumentNullException(nameof(key));
            _tokenprovider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
            
            FlurlHttp.Configure(settings => {
                settings.HttpClientFactory = new HttpClientFactory();
            });
        }

        public async Task AddCustomLogJsonAsync(string logName, object input, string timefield)
        {
            var json = JsonConvert.SerializeObject(input);

            // Create a hash for the API signature
            var datestring = DateTime.UtcNow.ToString("r", CultureInfo.CurrentCulture);
            var jsonBytes = Encoding.UTF8.GetBytes(json);
            var stringToHash = "POST\n" + jsonBytes.Length + "\napplication/json\n" + "x-ms-date:" + datestring + "\n/api/logs";
            var hashedString = BuildSignature(stringToHash, _key);
            var signature = "SharedKey " + _workspace + ":" + hashedString;

            await PostDataAsync(logName, signature, datestring, json, timefield).ConfigureAwait(false);
        }

        public async Task<LogAnalyticsQueryResponse> QueryAsync(string query)
        {
            var token = await _tokenprovider.GetAccessTokenAsync()
                .ConfigureAwait(false);
            var url = $"https://api.loganalytics.io/v1/workspaces/{_workspace}/query";

            try
            {
                return await url
                    .WithOAuthBearerToken(token)
                    .PostJsonAsync(new LogAnalyticsQuery { query = query })
                    .ReceiveJson<LogAnalyticsQueryResponse>()
                    .ConfigureAwait(false);
            }
            catch (FlurlHttpException ex)
            {
                if (ex.Call.HttpStatus == System.Net.HttpStatusCode.BadRequest)
                    return null;
                else
                    throw;
            }
        }

        // Build the API signature
        private static string BuildSignature(string message, string secret)
        {
            var encoding = new ASCIIEncoding();
            byte[] keyByte = Convert.FromBase64String(secret);
            byte[] messageBytes = encoding.GetBytes(message);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hash = hmacsha256.ComputeHash(messageBytes);
                return Convert.ToBase64String(hash);
            }
        }

        // Send a request to the POST API endpoint
        private async Task PostDataAsync(string logname, string signature, string date, string json, string timefield)
        {
            var url = "https://" + _workspace + ".ods.opinsights.azure.com/api/logs?api-version=2016-04-01";

            var content = new StringContent(json, Encoding.UTF8);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            (await url
                .WithHeader("Authorization", signature)
                .WithHeader("Log-Type", logname)
                .WithHeader("x-ms-date", date)
                .WithHeader("time-generated-field", timefield)
                .PostAsync(content)
                .ConfigureAwait(false))
                .Dispose();
        }
    }
}