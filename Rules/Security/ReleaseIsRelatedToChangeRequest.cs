using SecurePipelineScan.VstsService;
using System.Linq;
 

namespace SecurePipelineScan.Rules.Security
{
    public class ReleaseIsRelatedToChangeRequest : IRule
    {
        private readonly IVstsRestClient _client;

        public ReleaseIsRelatedToChangeRequest(IVstsRestClient client)
        {
            _client = client;
        }
        
        public string Description => "Release is related to a change request in SM9";
        
        public string Why => "To create traceability for audit, every release should have a tag with the corresponding SM9 change id";
        
        public bool Evaluate(string project, string releaseId)
        {
            var releases = _client.Get(VstsService.Requests.Release.Releases(project, releaseId));

            return releases.Tags.Any(x => x.Contains("SM9ChangeId"));
        }
    }
}