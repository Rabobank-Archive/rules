using Xunit;
using Shouldly;
using SecurePipelineScan.VstsService.Requests;
using Task = System.Threading.Tasks.Task;

namespace SecurePipelineScan.VstsService.Tests
{
    [Trait("category", "integration")]
    public class ReleaseSettings : IClassFixture<TestConfig>
    {
        private readonly IVstsRestClient _client;
        private readonly string _project;


        public ReleaseSettings(TestConfig config)
        {
            _client = new VstsRestClient(config.Organization, config.Token);
            _project = config.Project;
        }


        [Fact]
        public async Task CheckForCredentialsAndOtherSecrets()
        {
            var settings = await _client.GetAsync(ReleaseManagement.Settings(_project));
            settings.ComplianceSettings.CheckForCredentialsAndOtherSecrets.ShouldBeTrue();
        }
    }
}