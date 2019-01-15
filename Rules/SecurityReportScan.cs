using System;
using System.Collections.Generic;
using Rules.Reports;
using SecurePipelineScan.Rules.Checks;
using SecurePipelineScan.Rules.Reports;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Requests;
using SecurePipelineScan.VstsService.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using ApplicationGroup = SecurePipelineScan.VstsService.Requests.ApplicationGroup;
using Permission = SecurePipelineScan.Rules.Checks.Permission;
using Project = SecurePipelineScan.VstsService.Requests.Project;
using Release = SecurePipelineScan.VstsService.Requests.Release;
using Repository = SecurePipelineScan.VstsService.Response.Repository;
using SecurityNamespace = SecurePipelineScan.VstsService.Requests.SecurityNamespace;

namespace SecurePipelineScan.Rules
{
    public class SecurityReportScan : IProjectScan<SecurityReport>
    {
        private readonly IVstsRestClient client;

        public SecurityReportScan(IVstsRestClient client)
        {
            this.client = client;
        }

        public SecurityReport Execute(string project, DateTime date)
        {
            var applicationGroups =
                client.Get(ApplicationGroup.ApplicationGroups(project)).Identities;

            var groupMembers = getGroupMembersFromApplicationGroup(project, applicationGroups);

            var namespaceIdGitRepositories = client.Get(SecurityNamespace.SecurityNamespaces()).Value
                .First(ns => ns.DisplayName == "Git Repositories").NamespaceId;

            var namespaceIdBuild = client.Get(SecurityNamespace.SecurityNamespaces()).Value
                .First(ns => ns.Name == "Build").NamespaceId;

            var namespaceIdRelease = client.Get(SecurityNamespace.SecurityNamespaces()).Value
                .First(ns => ns.Name == "ReleaseManagement" &&
                             ns.Actions.
                                 Any(action => action.Name.Equals("ViewReleaseDefinition"))).NamespaceId;

            var applicationGroupIdProjectAdmins = applicationGroups
                .First(gi => gi.DisplayName == $"[{project}]\\Project Administrators").TeamFoundationId;

            var applicationGroupIdBuildAdmins = applicationGroups
                .First(gi => gi.DisplayName == $"[{project}]\\Build Administrators").TeamFoundationId;

            var applicationGroupIdReleaseAdmins = applicationGroups
                .First(gi => gi.DisplayName == $"[{project}]\\Release Administrators").TeamFoundationId;

            var applicationGroupIdProdEnvOwners = applicationGroups
                .First(gi => gi.DisplayName == $"[{project}]\\Production Environment Owners").TeamFoundationId;

            var applicationGroupRabobankProjectAdministrators = applicationGroups
                .First(gi => gi.DisplayName == $"[{project}]\\Rabobank Project Administrators").TeamFoundationId;

            var applicationGroupIdContributors = applicationGroups
                .First(gi => gi.DisplayName == $"[{project}]\\Contributors").TeamFoundationId;

            var projectId = client.Get(Project.Properties(project)).Id;

            var permissionsGitRepositorySet = client.Get(Permissions.PermissionsGroupRepositorySet(
                projectId, namespaceIdGitRepositories, applicationGroupIdProjectAdmins));

            var permissionsBuildProjectAdmins = client.Get(Permissions.PermissionsGroupSetId(
                projectId, namespaceIdBuild, applicationGroupIdProjectAdmins));

            var permissionsBuildBuildAdmins = client.Get(Permissions.PermissionsGroupSetId(
                projectId, namespaceIdBuild, applicationGroupIdBuildAdmins));

            var permissionsTeamRabobankProjectAdministrators = client.Get(Permissions.PermissionsGroupProjectId(
                projectId, applicationGroupRabobankProjectAdministrators));

            var permissionsReleaseRabobankProjectAdmininistrators = client.Get(Permissions.PermissionsGroupSetId(
                projectId, namespaceIdRelease, applicationGroupRabobankProjectAdministrators));

            var permissionsReleaseProdEnvOwner = client.Get(Permissions.PermissionsGroupSetId(
                projectId, namespaceIdRelease, applicationGroupIdProdEnvOwners));

            var permissionsReleaseContributors = client.Get(Permissions.PermissionsGroupSetId(
                projectId, namespaceIdRelease, applicationGroupIdContributors));

            var repositories = client.Get(VstsService.Requests.Repository.Repositories(projectId)).Value;

            var buildDefinitions = client.Get(Builds.BuildDefinitions(projectId)).Value;

            var releaseDefinitions = client.Get((Release.Definitions(projectId))).Value;

            
            var securityReport = new SecurityReport(date)
            {
                Project = project,

                ApplicationGroupContainsProductionEnvironmentOwner =
                    ProjectApplicationGroup.ApplicationGroupContainsProductionEnvironmentOwner(applicationGroups),
                ProjectAdminGroupOnlyContainsRabobankProjectAdminGroup =
                    ProjectApplicationGroup.ProjectAdministratorsGroupOnlyContainsRabobankProjectAdministratorsGroup(groupMembers),

                RepositoryRightsProjectAdmin = CheckRepositoryRights(permissionsGitRepositorySet.Permissions,
                            repositories, projectId, namespaceIdGitRepositories, applicationGroupIdProjectAdmins),

                BuildRightsBuildAdmin = CheckBuildRights(permissionsBuildBuildAdmins.Permissions),
                BuildRightsProjectAdmin = CheckBuildRights(permissionsBuildProjectAdmins.Permissions),

                BuildDefinitionsRightsBuildAdmin = CheckBuildDefinitionRights(buildDefinitions, projectId, namespaceIdBuild, applicationGroupIdBuildAdmins),
                BuildDefinitionsRightsProjectAdmin = CheckBuildDefinitionRights(buildDefinitions, projectId, namespaceIdBuild, applicationGroupIdProjectAdmins),

                ReleaseRightsProductionEnvOwner = CheckReleaseRightsProdEnvOwner(permissionsReleaseProdEnvOwner.Permissions, new ProductionEnvOwnerReleaseRights()),
                ReleaseRightsRaboProjectAdmin = CheckReleaseRights(permissionsReleaseRabobankProjectAdmininistrators.Permissions, new RaboAdminReleaseRights()),
                ReleaseRightsContributor = CheckReleaseRights(permissionsReleaseContributors.Permissions, new ContributorsReleaseRights()),

                ReleaseDefintionsRightsProductionEnvOwner = CheckReleaseDefinitionRightsProdEnvOwner(releaseDefinitions, projectId, namespaceIdRelease, applicationGroupIdProdEnvOwners, new ProductionEnvOwnerReleaseRights()),
                ReleaseDefintionsRightsContributor = CheckReleaseDefinitionRights(releaseDefinitions, projectId, namespaceIdRelease, applicationGroupIdContributors, new ContributorsReleaseRights()),
                ReleaseDefinitionsRightsRaboProjectAdmin = CheckReleaseDefinitionRights(releaseDefinitions, projectId, namespaceIdRelease, applicationGroupIdProjectAdmins, new RaboAdminReleaseRights()),
                
                TeamRabobankProjectAdministrators = CheckTeamRabobankProjectAdministrators(permissionsTeamRabobankProjectAdministrators.Security.Permissions),
            };

            return securityReport;
        }

        private GlobalRights CheckTeamRabobankProjectAdministrators(IEnumerable<SecurePipelineScan.VstsService.Response.Permission> permissions)
        {
            return new GlobalRights
            {
                HasNoPermissionToDeleteTeamProject =
                    Permission.HasNoPermissionToDeleteTeamProject(permissions),
                HasNoPermissionToPermanentlyDeleteWorkitems =
                    Permission.HasNoPermissionToPermanentlyDeleteWorkitems(permissions),
                HasNoPermissionToManageProjectProperties =
                    Permission.HasNoPermissionToManageProjectProperties(permissions),
            };
        }

        private RepositoryRights CheckRepositoryRights(
            IEnumerable<SecurePipelineScan.VstsService.Response.Permission> permissions,
            IEnumerable<Repository> repositories, string projectId, string namespaceId, string applicationGroupId)
        {
            return new RepositoryRights
            {
                HasNoPermissionToDeleteRepositories =
                    ApplicationGroupHasNoPermissionToDeleteRepositories(repositories, projectId, namespaceId, applicationGroupId),
                HasNoPermissionToDeleteRepositorySet =
                    Permission.HasNoPermissionToDeleteRepository(permissions),
                HasNotSetToManagePermissionsRepositories =
                    ApplicationGroupHasNotSetToManagePermissionsRepositories(repositories, projectId, namespaceId, applicationGroupId),
                HasNotSetToManagePermissionsRepositorySet =
                    Permission.HasNotSetToManageRepositoryPermissions(permissions)
            };
        }

        private BuildRights CheckBuildDefinitionRights(
            IEnumerable<BuildDefinition> buildDefinitions, string projectId, string namespaceId, string applicationGroupId)
        {
            return new BuildRights
            {
                HasNoPermissionsToDeleteBuilds =
                    ApplicationGroupHasNoPermissionToDeleteBuilds(buildDefinitions, projectId, namespaceId,
                        applicationGroupId),
                HasNoPermissionsToDeDestroyBuilds =
                    ApplicationGroupHasNoPermissionToDestroyBuilds(buildDefinitions, projectId, namespaceId,
                        applicationGroupId),
                HasNoPermissionsToDeleteBuildDefinition =
                    ApplicationGroupHasNoPermissionToDeleteBuildDefinition(buildDefinitions, projectId, namespaceId,
                        applicationGroupId),
                HasNoPermissionsToAdministerBuildPermissions =
                    ApplicationGroupHasNoPermissionToAdministerBuildPermissions(buildDefinitions, projectId, namespaceId,
                        applicationGroupId),
            };
        }

        private BuildRights CheckBuildRights(
            IEnumerable<VstsService.Response.Permission> permissions)
        {
            return new BuildRights
            {
                HasNoPermissionsToAdministerBuildPermissions =
                    Permission.HasNoPermissionToAdministerBuildPermissions(permissions),
                HasNoPermissionsToDeleteBuilds =
                    Permission.HasNoPermissionToDeleteBuilds(permissions),
                HasNoPermissionsToDeDestroyBuilds =
                    Permission.HasNoPermissionToDestroyBuilds(permissions),
                HasNoPermissionsToDeleteBuildDefinition =
                    Permission.HasNoPermissionToDeleteBuildDefinition(permissions)
            };
        }

        private ReleaseRights CheckReleaseRights(
            IEnumerable<VstsService.Response.Permission> permissions,
            ReleaseRights releaseRights)
        {
            releaseRights.HasNoPermissionToCreateReleases =
                Permission.HasNoPermissionToCreateReleases(permissions);
            releaseRights.HasPermissionToCreateReleases =
                Permission.HasPermissionToCreateReleases(permissions);
            releaseRights.HasNotSetToManageReleaseApprovers =
                Permission.HasNotSetToManageReleaseApprovers(permissions);

            releaseRights.HasNoPermissionToDeleteReleases =
                Permission.HasNoPermissionToDeleteReleases(permissions);
            releaseRights.HasNoPermissionToManageReleaseApprovers =
                Permission.HasNoPermissionToManageReleaseApprovers(permissions);
            releaseRights.HasNoPermissionsToAdministerReleasePermissions =
                Permission.HasNoPermissionToAdministerReleasePermissions(permissions);
            releaseRights.HasNoPermissionToDeleteReleasePipeline =
                Permission.HasNoPermissionToDeleteReleasePipeline(permissions);
            return releaseRights;
        }

        private ReleaseRights CheckReleaseRightsProdEnvOwner(
            IEnumerable<VstsService.Response.Permission> permissions, ReleaseRights releaseRights)
        {
            releaseRights.HasNoPermissionToCreateReleases = Permission.HasNoPermissionToCreateReleases(permissions);
            releaseRights.HasPermissionToManageReleaseApprovers =
                Permission.HasPermissionToManageReleaseApprovers(permissions);
            
            return releaseRights;
        }
        
        private ReleaseRights CheckReleaseDefinitionRights(
            IEnumerable<ReleaseDefinition> releaseDefinitions, string projectId, string namespaceId, string applicationGroupId, ReleaseRights releaseRights)
        {
            releaseRights.HasNoPermissionToCreateReleases =
                ApplicationGroupHasNoPermissionsToCreateReleases(releaseDefinitions, projectId, namespaceId,
                    applicationGroupId);
            releaseRights.HasNoPermissionToDeleteReleases =
                ApplicationGroupHasNoPermissionsToDeleteReleases(releaseDefinitions, projectId, namespaceId,
                    applicationGroupId);
            releaseRights.HasNoPermissionToDeleteReleasePipeline =
                ApplicationGroupHasNoPermissionsToDeleteReleasePipeline(releaseDefinitions, projectId, namespaceId,
                    applicationGroupId);
            releaseRights.HasNoPermissionToManageReleaseApprovers =
                ApplicationGroupHasNoPermissionsToManageReleaseApprovers(releaseDefinitions, projectId, namespaceId,
                    applicationGroupId);
            return releaseRights;
        }

        private ReleaseRights CheckReleaseDefinitionRightsProdEnvOwner(
            IEnumerable<ReleaseDefinition> releaseDefinitions, string projectId, string namespaceId, string applicationGroupId, ReleaseRights releaseRights)
        {
            releaseRights.HasNoPermissionToCreateReleases =
                ApplicationGroupHasNoPermissionsToCreateReleases(releaseDefinitions, projectId, namespaceId,
                    applicationGroupId);
            releaseRights.HasPermissionToManageReleaseApprovers =
                ApplicationGroupHasPermissionsToManageReleaseApprovers(releaseDefinitions, projectId, namespaceId,
                    applicationGroupId);
            return releaseRights;
        }

        
        private IEnumerable<VstsService.Response.ApplicationGroup> getGroupMembersFromApplicationGroup(string project, IEnumerable<VstsService.Response.ApplicationGroup> applicationGroups)
        {
            var groupId = applicationGroups.Single(x => x.DisplayName == $"[{project}]\\Project Administrators").TeamFoundationId;
            return client.Get(ApplicationGroup.GroupMembers(project, groupId)).Identities;
        }

        private bool ApplicationGroupHasNotSetToManagePermissionsRepositories(IEnumerable<Repository> repositories, string projectId, string namespaceId, string applicationGroupId)
        {
            return repositories.All
            (r =>
                Permission.HasNotSetToManageRepositoryPermissions(
                    client.Get(Permissions.PermissionsGroupRepository(
                            projectId, namespaceId, applicationGroupId, r.Id))
                        .Permissions));
        }

        private bool ApplicationGroupHasNoPermissionToDeleteRepositories(IEnumerable<Repository> repositories, string projectId, string namespaceId, string applicationGroupId)
        {
            return repositories.All
            (r =>
                Permission.HasNoPermissionToDeleteRepository(
                    client.Get(Permissions.PermissionsGroupRepository(
                            projectId, namespaceId, applicationGroupId, r.Id))
                        .Permissions));
        }

        private bool ApplicationGroupHasNoPermissionToDeleteBuilds(IEnumerable<BuildDefinition> buildDefinitions, string projectId, string namespaceId, string applicationGroupId)
        {
            return buildDefinitions.All
            (r =>
                Permission.HasNoPermissionToDeleteBuilds(
                    client.Get(Permissions.PermissionsGroupSetIdDefinition(
                            projectId, namespaceId, applicationGroupId, r.id))
                        .Permissions));
        }

        private bool ApplicationGroupHasNoPermissionToDeleteBuildDefinition(IEnumerable<BuildDefinition> buildDefinitions, string projectId, string namespaceId, string applicationGroupId)
        {
            return buildDefinitions.All
            (r =>
                Permission.HasNoPermissionToDeleteBuildDefinition(
                    client.Get(Permissions.PermissionsGroupSetIdDefinition(
                            projectId, namespaceId, applicationGroupId, r.id))
                        .Permissions));
        }

        private bool ApplicationGroupHasNoPermissionToDestroyBuilds(IEnumerable<BuildDefinition> buildDefinitions, string projectId, string namespaceId, string applicationGroupId)
        {
            return buildDefinitions.All
            (r =>
                Permission.HasNoPermissionToDestroyBuilds(
                    client.Get(Permissions.PermissionsGroupSetIdDefinition(
                            projectId, namespaceId, applicationGroupId, r.id))
                        .Permissions));
        }

        private bool ApplicationGroupHasNoPermissionToAdministerBuildPermissions(IEnumerable<BuildDefinition> buildDefinitions, string projectId, string namespaceId, string applicationGroupId)
        {
            return buildDefinitions.All
            (r =>
                Permission.HasNoPermissionToAdministerBuildPermissions(
                    client.Get(Permissions.PermissionsGroupSetIdDefinition(
                            projectId, namespaceId, applicationGroupId, r.id))
                        .Permissions));
        }

        private bool ApplicationGroupHasNoPermissionsToAdministerReleasePermissions(IEnumerable<ReleaseDefinition> releaseDefinitions, string projectId, string namespaceId, string applicationGroupId)
        {
            return releaseDefinitions.All
            (r =>
                Permission.HasNoPermissionToAdministerReleasePermissions(
                    client.Get(Permissions.PermissionsGroupSetIdDefinition(
                            projectId, namespaceId, applicationGroupId, r.Id))
                        .Permissions));
        }

        private bool ApplicationGroupHasNoPermissionsToDeleteReleasePipeline(IEnumerable<ReleaseDefinition> releaseDefinitions, string projectId, string namespaceId, string applicationGroupId)
        {
            return releaseDefinitions.All
            (r =>
                Permission.HasNoPermissionToDeleteReleasePipeline(
                    client.Get(Permissions.PermissionsGroupSetIdDefinition(
                            projectId, namespaceId, applicationGroupId, r.Id))
                        .Permissions));
        }

        private bool ApplicationGroupHasNoPermissionsToDeleteReleases(IEnumerable<ReleaseDefinition> releaseDefinitions, string projectId, string namespaceId, string applicationGroupId)
        {
            return releaseDefinitions.All
            (r =>
                Permission.HasNoPermissionToDeleteReleases(
                    client.Get(Permissions.PermissionsGroupSetIdDefinition(
                            projectId, namespaceId, applicationGroupId, r.Id))
                        .Permissions));
        }

        private bool ApplicationGroupHasPermissionsToManageReleaseApprovers(IEnumerable<ReleaseDefinition> releaseDefinitions, string projectId, string namespaceId, string applicationGroupId)
        {
            return releaseDefinitions.All
            (r =>
                Permission.HasPermissionToManageReleaseApprovers(
                    client.Get(Permissions.PermissionsGroupSetIdDefinition(
                            projectId, namespaceId, applicationGroupId, r.Id))
                        .Permissions));
        }

        private bool ApplicationGroupHasNoPermissionsToManageReleaseApprovers(IEnumerable<ReleaseDefinition> releaseDefinitions, string projectId, string namespaceId, string applicationGroupId)
        {
            return releaseDefinitions.All
            (r =>
                Permission.HasNoPermissionToManageReleaseApprovers(
                    client.Get(Permissions.PermissionsGroupSetIdDefinition(
                            projectId, namespaceId, applicationGroupId, r.Id))
                        .Permissions));
        }

        private bool ApplicationGroupHasNoPermissionsToCreateReleases(IEnumerable<ReleaseDefinition> releaseDefinitions, string projectId, string namespaceId, string applicationGroupId)
        {
            return releaseDefinitions.All
            (r =>
                Permission.HasNoPermissionToCreateReleases(
                    client.Get(Permissions.PermissionsGroupSetIdDefinition(
                            projectId, namespaceId, applicationGroupId, r.Id))
                        .Permissions));
        }

        
    }
}