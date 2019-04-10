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


        public string Description => "Only Project Administrators can delete the Team Project";

        public bool Evaluate(string project)
        {
            var idProjectAdministrators = _client.Get(VstsService.Requests.ApplicationGroup.ApplicationGroups(project))
                .Identities.Single(p => p.FriendlyDisplayName == "Project Administrators").TeamFoundationId;
            
            var groupMembersProjectAdministrators = _client.Get(VstsService.Requests.ApplicationGroup.GroupMembers(project, idProjectAdministrators)).Identities;

            return 
                groupMembersProjectAdministrators.Count(m => m.FriendlyDisplayName == "Rabobank Project Administrators") <= 1 && 
                groupMembersProjectAdministrators.Count() <= 1;
           
        }
    }
}