using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace SecurePipelineScan.VstsService.Tests
{
    public class SocketDepletedTests:  IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;
        private readonly ITestOutputHelper _testOutputHelper;
        private IVstsRestClient _vstsRestClient;
        private HttpClient _httpClient;

        public SocketDepletedTests(TestConfig config, ITestOutputHelper testOutputHelper)
        {
            _config = config;
            _testOutputHelper = testOutputHelper;
            _vstsRestClient = new VstsRestClient(_config.Organization, _config.Token);
            _httpClient = new HttpClient();
        }
        
       // [Fact]
        public void RestSharpDepletesSockets()
        {
            for (var i = 0; i < 100; i++)
            {
                var build = _vstsRestClient.Get(Requests.Builds.Build(_config.Project, _config.BuildId));
            }
            
            _testOutputHelper.WriteLine($"Number of open sockets: {CountWaitingConnections()}");
        }

        //[Fact]
        public async Task HttpClientDepletesSockets()
        {
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(
                    System.Text.ASCIIEncoding.ASCII.GetBytes(
                        string.Format("{0}:{1}", "", _config.Token))));

            for (var i = 0; i < 100; i++)
            {
                using (HttpResponseMessage response = await _httpClient.GetAsync(
                    $"https://dev.azure.com/{_config.Organization}/_apis/projects"))
                {
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                }
            }
           
            _testOutputHelper.WriteLine($"Number of open sockets: {CountWaitingConnections()}");
        }
        
        public static int CountWaitingConnections()
        {
            IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
            TcpConnectionInformation[] connections = properties.GetActiveTcpConnections();
            int waitingConnections = 0;
    
            foreach (TcpConnectionInformation t in connections)
            {
                if (t.State == TcpState.TimeWait)
                {
                    waitingConnections++;
                }
       
            }

            return waitingConnections;
        }
    }
}