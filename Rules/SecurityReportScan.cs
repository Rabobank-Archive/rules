using System;
using System.Collections.Generic;
using Rules.Reports;
using SecurePipelineScan.Rules.Checks;
using SecurePipelineScan.Rules.Reports;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Requests;
using SecurePipelineScan.VstsService.Response;
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
                client.Get(ApplicationGroup.ApplicationGroups(project))
                    ?.Identities;
              
            var groupMembers = (project != null && applicationGroups != null)
                ? getGroupMembersFromApplicationGroup(project, applicationGroups)
                : null;

            var namespaceIdGitRepositories = client.Get(SecurityNamespace.SecurityNamespaces()).Value
                .FirstOrDefault(ns => ns.DisplayName == "Git Repositories")
                ?.NamespaceId;

            var namespaceIdBuild = client.Get(SecurityNamespace.SecurityNamespaces()).Value
                .FirstOrDefault(ns => ns.Name == "Build")
                ?.NamespaceId;

            var namespaceIdRelease = client.Get(SecurityNamespace.SecurityNamespaces()).Value
                .FirstOrDefault(ns => ns.Name == "ReleaseManagement" &&
                             ns.Actions.
                                 Any(action => action.Name.Equals("ViewReleaseDefinition")))
                ?.NamespaceId;

            var applicationGroupIdProjectAdmins = applicationGroups
                .FirstOrDefault(gi => gi.DisplayName == $"[{project}]\\Project Administrators")
                ?.TeamFoundationId;

            var applicationGroupIdBuildAdmins = applicationGroups
                .FirstOrDefault(gi => gi.DisplayName == $"[{project}]\\Build Administrators")
                ?.TeamFoundationId;

            var applicationGroupIdReleaseAdmins = applicationGroups
                .FirstOrDefault(gi => gi.DisplayName == $"[{project}]\\Release Administrators")
                ?.TeamFoundationId;

            var applicationGroupIdProdEnvOwners = applicationGroups
                .FirstOrDefault(gi => gi.DisplayName == $"[{project}]\\Production Environment Owners")
                ?.TeamFoundationId;

            var applicationGroupRabobankProjectAdministrators = applicationGroups
                .FirstOrDefault(gi => gi.DisplayName == $"[{project}]\\Rabobank Project Administrators")
                ?.TeamFoundationId;

            var applicationGroupIdContributors = applicationGroups
                .FirstOrDefault(gi => gi.DisplayName == $"[{project}]\\Contributors")
                ?.TeamFoundationId;

            
            var projectId = 
                project != null 
                    ? client.Get(Project.Properties(project)).Id 
                    : null;
            
            var repositories =
                (projectId != null)
                    ? client.Get(VstsService.Requests.Repository.Repositories(projectId)).Value
                    : null;

            var buildDefinitions =
                (projectId != null)
                    ? client.Get(Builds.BuildDefinitions(projectId)).Value
                    : null;

            var releaseDefinitions =
                (projectId != null)
                    ? client.Get((Release.Definitions(projectId))).Value
                    : null;

            var permissionsGitRepositorySet =
                (projectId != null && namespaceIdGitRepositories != null && applicationGroupIdProjectAdmins != null)
                    ? client.Get(Permissions.PermissionsGroupRepositorySet(
                        projectId, namespaceIdGitRepositories, applicationGroupIdProjectAdmins))
                    : null;

            var permissionsBuildProjectAdmins =
                (projectId != null && namespaceIdBuild != null && applicationGroupIdProjectAdmins != null)
                    ? client.Get(Permissions.PermissionsGroupSetId(
                        projectId, namespaceIdBuild, applicationGroupIdProjectAdmins))
                    : null;

            var permissionsBuildBuildAdmins =
                (projectId != null && namespaceIdBuild != null && applicationGroupIdBuildAdmins != null)
                    ? client.Get(Permissions.PermissionsGroupSetId(
                        projectId, namespaceIdBuild, applicationGroupIdBuildAdmins))
                    : null;

            var permissionsBuildContributors =
                (projectId != null && namespaceIdBuild != null && applicationGroupIdContributors != null)
                    ? client.Get(Permissions.PermissionsGroupSetId(
                        projectId, namespaceIdBuild, applicationGroupIdContributors))
                    : null;

            var permissionsTeamRabobankProjectAdministrators =
                (projectId != null && applicationGroupRabobankProjectAdministrators != null)
                    ? client.Get(Permissions.PermissionsGroupProjectId(
                        projectId, applicationGroupRabobankProjectAdministrators))
                    : null;

            var permissionsReleaseRabobankProjectAdministrators =
                (projectId != null && namespaceIdRelease != null &&
                 applicationGroupRabobankProjectAdministrators != null)
                    ? client.Get(Permissions.PermissionsGroupSetId(
                        projectId, namespaceIdRelease, applicationGroupRabobankProjectAdministrators))
                    : null;

            var permissionsReleaseProdEnvOwner =
                (projectId != null && namespaceIdRelease != null && applicationGroupIdProdEnvOwners != null)
                    ? client.Get(Permissions.PermissionsGroupSetId(
                        projectId, namespaceIdRelease, applicationGroupIdProdEnvOwners))
                    : null;

            var permissionsReleaseContributors =
                (projectId != null && namespaceIdRelease != null && applicationGroupIdContributors != null)
                    ? client.Get(Permissions.PermissionsGroupSetId(
                        projectId, namespaceIdRelease, applicationGroupIdContributors))
                    : null;

            var permissionsReleaseProjectAdministrators =
                (projectId != null && namespaceIdRelease != null && applicationGroupIdProjectAdmins != null)
                    ? client.Get(Permissions.PermissionsGroupSetId(
                        projectId, namespaceIdRelease, applicationGroupIdProjectAdmins))
                    : null;

            var permissionsReleaseReleaseAdministrators =
                (projectId != null && namespaceIdRelease != null && applicationGroupIdReleaseAdmins != null)
                    ? client.Get(Permissions.PermissionsGroupSetId(
                        projectId, namespaceIdRelease, applicationGroupIdReleaseAdmins))
                    : null;

            
            var securityReport = new SecurityReport(date)
            {
                Project = project,

                ApplicationGroupContainsProductionEnvironmentOwner = 
                    applicationGroups != null && ProjectApplicationGroup.ApplicationGroupContainsProductionEnvironmentOwner(applicationGroups),
                ProjectAdminGroupOnlyContainsRabobankProjectAdminGroup = 
                    groupMembers != null && ProjectApplicationGroup.ProjectAdministratorsGroupOnlyContainsRabobankProjectAdministratorsGroup(groupMembers),

                RepositoryRightsProjectAdmin = 
                    (permissionsGitRepositorySet != null && repositories != null && projectId != null && namespaceIdGitRepositories != null && applicationGroupIdProjectAdmins != null)
                    ? CheckRepositoryRights(permissionsGitRepositorySet.Permissions,
                            repositories, projectId, namespaceIdGitRepositories, applicationGroupIdProjectAdmins)
                    : new RepositoryRights(),

                BuildRightsBuildAdmin = 
                    (permissionsBuildBuildAdmins != null)
                    ? CheckBuildRights(permissionsBuildBuildAdmins.Permissions, new BuildAdminBuildRights())
                    : new BuildAdminBuildRights(),
                BuildRightsProjectAdmin = 
                    (permissionsBuildProjectAdmins != null)
                    ? CheckBuildRights(permissionsBuildProjectAdmins.Permissions, new ProjectAdminBuildRights())
                    : new ProjectAdminBuildRights(),
                BuildRightsContributor = 
                    (permissionsBuildContributors != null)
                    ? CheckBuildRights(permissionsBuildContributors.Permissions, new ContributorsBuildRights())
                    : new ContributorsBuildRights(),
                
                BuildDefinitionsRightsBuildAdmin = 
                    (buildDefinitions != null && projectId != null && namespaceIdBuild != null && applicationGroupIdBuildAdmins != null)
                    ? CheckBuildDefinitionRights(buildDefinitions, projectId, namespaceIdBuild, applicationGroupIdBuildAdmins, new BuildAdminBuildRights())
                    : new BuildAdminBuildRights(),
                BuildDefinitionsRightsProjectAdmin = 
                    (buildDefinitions != null && projectId != null && namespaceIdBuild != null && applicationGroupIdProjectAdmins != null)
                    ? CheckBuildDefinitionRights(buildDefinitions, projectId, namespaceIdBuild, applicationGroupIdProjectAdmins, new ProjectAdminBuildRights())
                    : new ProjectAdminBuildRights(),
                BuildDefinitionsRightsContributor = 
                    (buildDefinitions != null && projectId != null && namespaceIdBuild != null && applicationGroupIdContributors != null)
                    ? CheckBuildDefinitionRights(buildDefinitions, projectId, namespaceIdBuild,applicationGroupIdContributors, new ContributorsBuildRights())
                    : new ContributorsBuildRights(),
                
                ReleaseRightsProductionEnvOwner = 
                    permissionsReleaseProdEnvOwner != null 
                        ? CheckReleaseRights(permissionsReleaseProdEnvOwner.Permissions, new ProductionEnvOwnerReleaseRights()) 
                        : new ProductionEnvOwnerReleaseRights(),
                ReleaseRightsRaboProjectAdmin = 
                    permissionsReleaseRabobankProjectAdministrators != null
                    ? CheckReleaseRights(permissionsReleaseRabobankProjectAdministrators.Permissions, new RaboAdminReleaseRights())
                    : new ProjectAdminReleaseRights(),
                ReleaseRightsContributor = 
                    permissionsReleaseContributors != null
                    ? CheckReleaseRights(permissionsReleaseContributors.Permissions, new ContributorsReleaseRights())
                    : new ContributorsReleaseRights(),
                ReleaseRightsProjectAdmin = 
                    permissionsReleaseProjectAdministrators != null
                    ? CheckReleaseRights(permissionsReleaseProjectAdministrators.Permissions, new ProjectAdminReleaseRights())
                    : new ProjectAdminReleaseRights(),
                ReleaseRightsReleaseAdmin = 
                    permissionsReleaseReleaseAdministrators != null
                    ? CheckReleaseRights(permissionsReleaseReleaseAdministrators.Permissions, new ProjectAdminReleaseRights())
                    : new ReleaseAdminReleaseRights(),

                ReleaseDefinitionsRightsProductionEnvOwner = 
                    (releaseDefinitions != null && projectId != null && namespaceIdRelease != null && applicationGroupIdProdEnvOwners != null)
                    ? CheckReleaseDefinitionRights(releaseDefinitions, projectId, namespaceIdRelease, applicationGroupIdProdEnvOwners, new ProductionEnvOwnerReleaseRights())
                    : new ProductionEnvOwnerReleaseRights(),
                ReleaseDefinitionsRightsContributor = 
                    (releaseDefinitions != null && projectId != null && namespaceIdRelease != null && applicationGroupIdContributors != null)
                    ? CheckReleaseDefinitionRights(releaseDefinitions, projectId, namespaceIdRelease, applicationGroupIdContributors, new ContributorsReleaseRights())
                    : new ContributorsReleaseRights(),
                ReleaseDefinitionsRightsRaboProjectAdmin = 
                    (releaseDefinitions != null && projectId != null && namespaceIdRelease != null && applicationGroupRabobankProjectAdministrators != null)
                    ? CheckReleaseDefinitionRights(releaseDefinitions, projectId, namespaceIdRelease, applicationGroupRabobankProjectAdministrators, new RaboAdminReleaseRights())
                    : new RaboAdminReleaseRights(),
                ReleaseDefinitionsRightsProjectAdmin = 
                    (releaseDefinitions != null && projectId != null && namespaceIdRelease != null && applicationGroupIdProjectAdmins != null)
                    ? CheckReleaseDefinitionRights(releaseDefinitions, projectId, namespaceIdRelease, applicationGroupIdProjectAdmins, new ProjectAdminReleaseRights())
                    : new ProjectAdminReleaseRights(),
                ReleaseDefinitionsRightsReleaseAdmin = 
                    (releaseDefinitions != null && projectId != null && namespaceIdRelease != null && applicationGroupIdReleaseAdmins != null)
                    ? CheckReleaseDefinitionRights(releaseDefinitions, projectId, namespaceIdRelease, applicationGroupIdReleaseAdmins, new ReleaseAdminReleaseRights())
                    : new ReleaseAdminReleaseRights(),
                
                TeamRabobankProjectAdministrators = 
                    permissionsTeamRabobankProjectAdministrators != null 
                    ? CheckTeamRabobankProjectAdministrators(permissionsTeamRabobankProjectAdministrators.Security.Permissions)
                    : new GlobalRights(),
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
            IEnumerable<BuildDefinition> buildDefinitions, string projectId, string namespaceId, string applicationGroupId, BuildRights buildRights)
        {
            buildRights.HasNoPermissionsToDeleteBuilds =
                ApplicationGroupHasNoPermissionToDeleteBuilds(buildDefinitions, projectId, namespaceId,
                    applicationGroupId);
            buildRights.HasNoPermissionsToDestroyBuilds =
                ApplicationGroupHasNoPermissionToDestroyBuilds(buildDefinitions, projectId, namespaceId,
                    applicationGroupId);
            buildRights.HasNoPermissionsToDeleteBuildDefinition =
                ApplicationGroupHasNoPermissionsToDeleteBuildDefinition(buildDefinitions, projectId, namespaceId,
                    applicationGroupId);
            buildRights.HasNoPermissionsToAdministerBuildPermissions =
                ApplicationGroupHasNoPermissionToAdministerBuildPermissions(buildDefinitions, projectId, namespaceId,
                    applicationGroupId);
            buildRights.HasNotSetToDeleteBuildDefinition =                 
                ApplicationGroupHasNotSetDeleteBuildDefinition(buildDefinitions, projectId, namespaceId,
                applicationGroupId);
            buildRights.HasNotSetToDeleteBuilds =
                ApplicationGroupHasNotSetToDeleteBuilds(buildDefinitions, projectId, namespaceId,
                    applicationGroupId);
            buildRights.HasNotSetToDestroyBuilds =
                ApplicationGroupHasNotSetToDestroyBuilds(buildDefinitions, projectId, namespaceId,
                    applicationGroupId);

            return buildRights;
        }

        private BuildRights CheckBuildRights(
            IEnumerable<VstsService.Response.Permission> permissions, BuildRights buildRights)
        {
            {
                buildRights.HasNoPermissionsToAdministerBuildPermissions =
                    Permission.HasNoPermissionToAdministerBuildPermissions(permissions);
                buildRights.HasNoPermissionsToDeleteBuilds =
                    Permission.HasNoPermissionToDeleteBuilds(permissions);
                buildRights.HasNoPermissionsToDestroyBuilds =
                    Permission.HasNoPermissionToDestroyBuilds(permissions);
                buildRights.HasNoPermissionsToDeleteBuildDefinition =
                    Permission.HasNoPermissionToDeleteBuildDefinition(permissions);
                buildRights.HasNotSetToDeleteBuildDefinition =
                    Permission.HasNotSetToDeleteBuildDefinition(permissions);
                buildRights.HasNotSetToDeleteBuilds =
                    Permission.HasNotSetToDeleteBuilds(permissions);
                buildRights.HasNotSetToDestroyBuilds =
                    Permission.HasNotSetToDestroyBuilds(permissions);
            };

            return buildRights;
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
            releaseRights.HasPermissionToManageReleaseApprovers =
                Permission.HasPermissionToManageReleaseApprovers(permissions);
            releaseRights.HasNoPermissionToDeleteReleases =
                Permission.HasNoPermissionToDeleteReleases(permissions);
            releaseRights.HasNoPermissionToManageReleaseApprovers =
                Permission.HasNoPermissionToManageReleaseApprovers(permissions);
            releaseRights.HasNoPermissionsToAdministerReleasePermissions =
                Permission.HasNoPermissionToAdministerReleasePermissions(permissions);
            releaseRights.HasNoPermissionToDeleteReleasePipeline =
                Permission.HasNoPermissionToDeleteReleasePipeline(permissions);
            releaseRights.HasNoPermissionToDeleteReleaseStage =
                Permission.HasNoPermissionToDeleteReleaseStage(permissions);
            releaseRights.HasPermissionToDeleteReleaseStage = 
                Permission.HasPermissionToDeleteReleaseStage(permissions);
            releaseRights.HasNotSetToDeleteReleaseStage =
                Permission.HasNotSetToDeleteReleaseStage(permissions);
            return releaseRights;
        }
        
        private ReleaseRights CheckReleaseDefinitionRights(
            IEnumerable<ReleaseDefinition> releaseDefinitions, string projectId, string namespaceId, string applicationGroupId, ReleaseRights releaseRights)
        {
            releaseRights.HasNoPermissionsToAdministerReleasePermissions = 
                ApplicationGroupHasNoPermissionsToAdministerReleasePermissions(releaseDefinitions, projectId, namespaceId,
                    applicationGroupId);
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
            releaseRights.HasPermissionToManageReleaseApprovers =
                ApplicationGroupHasPermissionsToManageReleaseApprovers(releaseDefinitions, projectId, namespaceId,
                    applicationGroupId);
            releaseRights.HasNotSetToManageReleaseApprovers =
                ApplicationGroupHasNotSetToManageReleaseApprovers(releaseDefinitions, projectId, namespaceId,
                    applicationGroupId);
            releaseRights.HasNoPermissionToDeleteReleaseStage = 
                ApplicationGroupHasNoPermissionToDeleteReleaseStage(releaseDefinitions, projectId, namespaceId,
                    applicationGroupId);
            releaseRights.HasNotSetToDeleteReleaseStage =
                ApplicationGroupHasNotSetToDeleteReleaseStage(releaseDefinitions, projectId, namespaceId,
                    applicationGroupId);
            releaseRights.HasPermissionToDeleteReleaseStage = 
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
                            projectId, namespaceId, applicationGroupId, r.Id))
                        .Permissions));
        }
        
        private bool ApplicationGroupHasNoPermissionsToDeleteBuildDefinition(IEnumerable<BuildDefinition> buildDefinitions, string projectId, string namespaceId, string applicationGroupId)
        {
            return buildDefinitions.All
            (r =>
                Permission.HasNoPermissionToDeleteBuildDefinition(
                    client.Get(Permissions.PermissionsGroupSetIdDefinition(
                            projectId, namespaceId, applicationGroupId, r.Id))
                        .Permissions));
        }
        
        private bool ApplicationGroupHasNotSetToDeleteBuilds(IEnumerable<BuildDefinition> buildDefinitions, string projectId, string namespaceId, string applicationGroupId)
        {
            return buildDefinitions.All
            (r =>
                Permission.HasNotSetToDeleteBuilds(
                    client.Get(Permissions.PermissionsGroupSetIdDefinition(
                            projectId, namespaceId, applicationGroupId, r.Id))
                        .Permissions));
        }

        
        private bool ApplicationGroupHasNotSetDeleteBuildDefinition(IEnumerable<BuildDefinition> buildDefinitions, string projectId, string namespaceId, string applicationGroupId)
        {
            return buildDefinitions.All
            (r =>
                Permission.HasNotSetToDeleteBuildDefinition(
                    client.Get(Permissions.PermissionsGroupSetIdDefinition(
                            projectId, namespaceId, applicationGroupId, r.Id))
                        .Permissions));
        }

        private bool ApplicationGroupHasNoPermissionToDestroyBuilds(IEnumerable<BuildDefinition> buildDefinitions, string projectId, string namespaceId, string applicationGroupId)
        {
            return buildDefinitions.All
            (r =>
                Permission.HasNoPermissionToDestroyBuilds(
                    client.Get(Permissions.PermissionsGroupSetIdDefinition(
                            projectId, namespaceId, applicationGroupId, r.Id))
                        .Permissions));
        }

        private bool ApplicationGroupHasNotSetToDestroyBuilds(IEnumerable<BuildDefinition> buildDefinitions, string projectId, string namespaceId, string applicationGroupId)
        {
            return buildDefinitions.All
            (r =>
                Permission.HasNotSetToDestroyBuilds(
                    client.Get(Permissions.PermissionsGroupSetIdDefinition(
                            projectId, namespaceId, applicationGroupId, r.Id))
                        .Permissions));
        }

        
        private bool ApplicationGroupHasNoPermissionToAdministerBuildPermissions(IEnumerable<BuildDefinition> buildDefinitions, string projectId, string namespaceId, string applicationGroupId)
        {
            return buildDefinitions.All
            (r =>
                Permission.HasNoPermissionToAdministerBuildPermissions(
                    client.Get(Permissions.PermissionsGroupSetIdDefinition(
                            projectId, namespaceId, applicationGroupId, r.Id))
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
        
        private bool ApplicationGroupHasNotSetToManageReleaseApprovers(IEnumerable<ReleaseDefinition> releaseDefinitions, string projectId, string namespaceId, string applicationGroupId)
        {
            return releaseDefinitions.All
            (r =>
                Permission.HasNotSetToManageReleaseApprovers(
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

        private bool ApplicationGroupHasNoPermissionToDeleteReleaseStage(
            IEnumerable<ReleaseDefinition> releaseDefinitions, string projectId, string namespaceId,
            string applicationGroupId)
        {
            return releaseDefinitions.All(
                r =>
                    Permission.HasNoPermissionToDeleteReleaseStage(
                        client.Get(Permissions.PermissionsGroupSetIdDefinition(
                                projectId, namespaceId, applicationGroupId, r.Id))
                            .Permissions));
        }

        private bool ApplicationGroupHasNotSetToDeleteReleaseStage(
            IEnumerable<ReleaseDefinition> releaseDefinitions, string projectId, string namespaceId,
            string applicationGroupId)
        {
            return releaseDefinitions.All(
                r =>
                    Permission.HasNotSetToDeleteReleaseStage(
                        client.Get(Permissions.PermissionsGroupSetIdDefinition(
                                projectId, namespaceId, applicationGroupId, r.Id))
                            .Permissions));
        }

    }
}