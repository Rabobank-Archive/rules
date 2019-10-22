using System.Threading.Tasks;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Requests;
using SecurePipelineScan.VstsService.Response;
using Task = System.Threading.Tasks.Task;

namespace SecurePipelineScan.Rules.Security
{
    public class ShouldBlockPlainTextCredentialsInPipelines : IProjectRule, IProjectReconcile
    {
        private readonly IVstsRestClient _client;

        public ShouldBlockPlainTextCredentialsInPipelines(IVstsRestClient client)
        {
            _client = client;
        }

        public string Description => "Plain text credentials are blocked in pipelines.";
        public string Link => null;
        public string[] Impact => new[]
        {
            "In project settings, 'Block release definition edits that contain plaintext " +
            "credentials or other secrets.' will be activated."
        };
        public bool IsSox => false;

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
                settings.ComplianceSettings = new ComplianceSettings();
            
            settings.ComplianceSettings.CheckForCredentialsAndOtherSecrets = true;
            
            await _client.PutAsync(ReleaseManagement.Settings(projectId), settings)
                .ConfigureAwait(false);
        }
    }
}