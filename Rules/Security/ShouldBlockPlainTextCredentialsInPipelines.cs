using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Requests;
using Response = SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.Rules.Security
{
    public class ShouldBlockPlainTextCredentialsInPipelines : IProjectRule, IProjectReconcile
    {
        private readonly IVstsRestClient _client;

        public ShouldBlockPlainTextCredentialsInPipelines(IVstsRestClient client)
        {
            _client = client;
        }

        [ExcludeFromCodeCoverage] public string Description => "Plain text credentials are blocked in pipelines.";
        [ExcludeFromCodeCoverage] public string Link => "https://confluence.dev.somecompany.nl/x/OxV1D";
        [ExcludeFromCodeCoverage] 
        public string[] Impact => new[]
        {
            "In project settings, 'Block release definition edits that contain plaintext " +
            "credentials or other secrets.' will be activated."
        };

        public async Task<bool> EvaluateAsync(string projectId)
        {
            var settings = await _client.GetAsync(ReleaseManagement.Settings(projectId))
                .ConfigureAwait(false);
            return settings.ComplianceSettings != null &&
                   settings.ComplianceSettings.CheckForCredentialsAndOtherSecrets;
        }

        public async Task ReconcileAsync(string projectId)
        {
            var settings = await _client.GetAsync(ReleaseManagement.Settings(projectId))
                .ConfigureAwait(false);
            if (settings.ComplianceSettings == null)
                settings.ComplianceSettings = new Response.ComplianceSettings();
            
            settings.ComplianceSettings.CheckForCredentialsAndOtherSecrets = true;
            
            await _client.PutAsync(ReleaseManagement.Settings(projectId), settings)
                .ConfigureAwait(false);
        }
    }
}