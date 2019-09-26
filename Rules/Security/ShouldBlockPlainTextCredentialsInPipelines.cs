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

        public string Why =>
            "To ensure that credentials are not leaked, it should not be allowed to store them in plain text.";

        public string[] Impact => new[]
        {
            "In project settings, 'Block release definition edits that contain plaintext credentials or other secrets.' will be activated."
        };
        public bool IsSox => false;

        public async Task<bool> EvaluateAsync(string project)
        {
            var settings = await _client.GetAsync(ReleaseManagement.Settings(project))
                .ConfigureAwait(false);
            return settings.ComplianceSettings != null &&
                   settings.ComplianceSettings.CheckForCredentialsAndOtherSecrets;
        }

        public Task<bool> EvaluateAsync(string project, string id)
        {
            throw new System.NotSupportedException();
        }

        public async Task ReconcileAsync(string project)
        {
            var settings = await _client.GetAsync(ReleaseManagement.Settings(project))
                .ConfigureAwait(false);
            if (settings.ComplianceSettings == null)
                settings.ComplianceSettings = new ComplianceSettings();
            
            settings.ComplianceSettings.CheckForCredentialsAndOtherSecrets = true;
            
            await _client.PutAsync(ReleaseManagement.Settings(project), settings)
                .ConfigureAwait(false);
        }
    }


}