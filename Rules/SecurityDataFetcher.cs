using Common;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Requests;
using System.Collections.Generic;
using System.Linq;

namespace SecurePipelineScan.Rules
{
    public class SecurityDataFetcher
    {
        private IVstsRestClient client;

        public SecurityDataFetcher(IVstsRestClient client)
        {
            this.client = client;
        }

        public IEnumerable<VstsService.Response.ApplicationGroup> applicationGroups { get; private set; }

        public ISecurityData FetchSecurityPermissions(string project)
        {
            SecurityData ProjectPermissions = new SecurityData(project);

            applicationGroups = client.Get(VstsService.Requests.ApplicationGroup.ApplicationGroups(project))
                    ?.Identities;

            ProjectPermissions.GlobalPermissions = FetchGlobalPermissions(project);

            return ProjectPermissions;
        }

        private IDictionary<string, IEnumerable<Permission>> FetchGlobalPermissions(string projectId)
        {
            var permissions = new Dictionary<string, IEnumerable<Permission>>();

            foreach (var applicationGroup in applicationGroups)
            {
                var permission = client.Get(Permissions.
                    PermissionsGroupProjectId(projectId, applicationGroup.TeamFoundationId));

                var applicationGroupName = applicationGroup.DisplayName.Replace(@"\", "");
                
                permissions.Add(applicationGroupName,
                    permission.Security.Permissions.Select(x =>
                    new Permission(x.PermissionBit, (PermissionId)x.PermissionId) { DisplayName = x.DisplayName}));
            };
            return permissions;
        }
    }
}