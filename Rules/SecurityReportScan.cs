using System.Collections;
using System.Collections.Generic;
using Rules.Reports;
using SecurePipelineScan.Rules.Checks;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Requests;
using System.Linq;
using Repository = SecurePipelineScan.VstsService.Response.Repository;

namespace SecurePipelineScan.Rules
{
    public class SecurityReportScan
    {
        private readonly IVstsRestClient client;

        public SecurityReportScan(IVstsRestClient client)
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
            var applicationGroups =
                client.Get(ApplicationGroup.ApplicationGroups(project)).Identities;
            
            var groupMembers = getGroupMembersFromApplicationGroup(project, applicationGroups);

            var namespaceId = client.Get(SecurityNamespace.SecurityNamespaces()).Value
                .First(ns => ns.DisplayName == "Git Repositories").NamespaceId;
                
            var applicationGroupId = applicationGroups
                .First(gi => gi.DisplayName == $"[{project}]\\Project Administrators").TeamFoundationId;
         
            var projectId = client.Get(Project.Properties(project)).Id;           
            
            var permissionsGitRepositorySet = client.Get(PermissionsGroupRepositories.PermissionsGroupRepositorySet(
                projectId, namespaceId, applicationGroupId));
            
            var repositories = client.Get(VstsService.Requests.Repository.Repositories(projectId)).Value;

            
            var securityReport = new SecurityReport
            {
                Project = project,
                ApplicationGroupContainsProductionEnvironmentOwner =
                    ProjectApplicationGroup.ApplicationGroupContainsProductionEnvironmentOwner(applicationGroups),
                
                ProjectAdminHasNoPermissionToDeleteRepositorySet = 
                    Permission.HasNoPermissionToDeleteRepository(permissionsGitRepositorySet.Permissions),
                ProjectAdminHasNoPermissionToDeleteRepositories = 
                    ProjectAdminHasNoPermissionToDeleteRepositories(repositories, projectId, namespaceId, applicationGroupId),
                
                ProjectAdminHasNoPermissionToManagePermissionsRepositorySet = 
                    Permission.HasNoPermissionToManageRepositoryPermissions(permissionsGitRepositorySet.Permissions),
                ProjectAdminHasNoPermissionToManagePermissionsRepositories = 
                    ProjectAdminHasNoPermissionToManagePermissionsRepositories(repositories, projectId, namespaceId, applicationGroupId),
                    
                ProjectAdminGroupOnlyContainsRabobankProjectAdminGroup = ProjectApplicationGroup.ProjectAdministratorsGroupOnlyContainsRabobankProjectAdministratorsGroup(groupMembers)
                    
            };

            securityReport.ProjectIsSecure = (
                securityReport.ApplicationGroupContainsProductionEnvironmentOwner &&
                securityReport.ProjectAdminHasNoPermissionToDeleteRepositories &&
                securityReport.ProjectAdminHasNoPermissionToDeleteRepositorySet &&
                securityReport.ProjectAdminHasNoPermissionToManagePermissionsRepositories &&
                securityReport.ProjectAdminHasNoPermissionToManagePermissionsRepositorySet);

           return securityReport;
        }

        private IEnumerable<VstsService.Response.ApplicationGroup> getGroupMembersFromApplicationGroup(string project, IEnumerable<VstsService.Response.ApplicationGroup> applicationGroups)
        {
            var groupId = applicationGroups.Single(x => x.DisplayName == $"[{project}]\\Project Administrators").TeamFoundationId;
            var groupMembers = client.Get(ApplicationGroup.GroupMembers(project, groupId)).Identities;
            return groupMembers;
        }

        private bool ProjectAdminHasNoPermissionToManagePermissionsRepositories(IEnumerable<Repository> repositories, string projectId, string namespaceId, string applicationGroupId)
        {
            return repositories.All
            (r => 
                Permission.HasNoPermissionToManageRepositoryPermissions(
                    client.Get(PermissionsGroupRepositories.PermissionsGroupRepository(
                            projectId, namespaceId, applicationGroupId, r.Id))
                        .Permissions)
                == true);
        }

        private bool ProjectAdminHasNoPermissionToDeleteRepositories(IEnumerable<Repository> repositories, string projectId, string namespaceId, string applicationGroupId)
        {
            return repositories.All
            (r => 
                Permission.HasNoPermissionToDeleteRepository(
                    client.Get(PermissionsGroupRepositories.PermissionsGroupRepository(
                            projectId, namespaceId, applicationGroupId, r.Id))
                        .Permissions)
                == true);
        }
    }
}