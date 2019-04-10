using System.Linq;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Requests;

namespace SecurePipelineScan.Rules
{
    public class ProjectAdministratorsOnlyContainRaboAdministrators : IProjectRule
    {
        private readonly VstsRestClient _client;

        public ProjectAdministratorsOnlyContainRaboAdministrators(VstsRestClient client)
        {
            _client = client;
        }


        public string Description => "Project Administrator Group only contains Rabobank Project Administrators";

        public bool Evaluate(string project)
        {
            var id = _client.Get(VstsService.Requests.ApplicationGroup.ApplicationGroups(project))
                .Identities.Single(p => p.FriendlyDisplayName == "Project Administrators").TeamFoundationId;
            
            var members = _client.Get(VstsService.Requests.ApplicationGroup.GroupMembers(project, id)).Identities;

            return
                members.All(m => m.FriendlyDisplayName == "Rabobank Project Administrators");

        }
    }
}