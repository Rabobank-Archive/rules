using System.Collections;
using System.Collections.Generic;
using Rules.Reports;
using SecurePipelineScan.Rules.Checks;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Requests;
using System.Linq;

namespace SecurePipelineScan.Rules
{
    public class SecurityReportScanRepositories
    {
        private readonly IVstsRestClient client;

        public SecurityReportScanRepositories(IVstsRestClient client)
        {
            this.client = client;
        }

        
        public void Execute()
        {
            var projects = client.Get(Project.Projects()).Value;
            foreach (var project in projects)
            {
                Execute(project.Name);
            }
        }


        public SecurityReport Execute(string project)
        {
            var nameSpaces = client.Get(Namespace.SecurityNamespaces()).Value;

            var queryNameSpaceId =
                from ns in nameSpaces
                where ns.DisplayName == "Git Repositories"
                select ns.NamespaceId;
            
            var groupIdentities = client.Get(ApplicationGroup.ApplicationGroups(project)).Identities;
            
            var queryApplicationGroupId = 
                from gi in groupIdentities
                where gi.DisplayName == $"[{project}]\\Project Administrators"
                select gi.TeamFoundationId;
         
            var projectId = client.Get(Project.ProjectProperties(project)).Id;
            
            
            var permissionsGitRepositorySet = client.Get(PermissionsGroupRepoSet.PermissionsGitRepositorySet(
                projectId, queryNameSpaceId.First(), queryApplicationGroupId.First()));
            
            var securityReport = new SecurityReport
            {
                ProjectAdminHasNoPermissionsToDeleteRepositorySet = 
                    Permission.HasNoPermissionToDeleteRepository(permissionsGitRepositorySet.Permissions),
                ProjectAdminHasNoPermissionToManagePermissionsRepositorySet = 
                    Permission.HasNoPermissionToManageRepositoryPermissions(permissionsGitRepositorySet.Permissions)
            };

            return securityReport;
        }

    }
}