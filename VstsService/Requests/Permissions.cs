using RestSharp;
using RestSharp.Extensions;
using SecurePipelineScan.VstsService.Response;
using System.Collections;

namespace SecurePipelineScan.VstsService.Requests
{
    public static class Permissions
    {
        public static IVstsRestRequest<Response.PermissionsSetId> PermissionsGroupRepositorySet(string projectId, string permissionSetId, string applicationGroupId)
        {
            return new VstsRestRequest<Response.PermissionsSetId>(
                $"{projectId}/_api/_security/DisplayPermissions?__v=5&tfid={applicationGroupId}&permissionSetId={permissionSetId}&permissionSetToken=repoV2%2F{projectId}");
        }
        
        public static IVstsRestRequest<Response.PermissionsSetId> PermissionsGroupRepository(string projectId, string permissionSetId, string applicationGroupId, string repositoryId)
        {
            return new VstsRestRequest<PermissionsSetId>(
                $"{projectId}/_api/_security/DisplayPermissions?__v=5&tfid={applicationGroupId}&permissionSetId={permissionSetId}&permissionSetToken=repoV2%2F{projectId}%2F{repositoryId}");
        }

        public static IVstsRestRequest<Response.PermissionsSetId> PermissionsGroupSetId(string projectId,string permissionSetId, string applicationGroupId)
        {
            return new VstsRestRequest<PermissionsSetId>(
                $"{projectId}/_api/_security/DisplayPermissions?__v=5&tfid={applicationGroupId}&permissionSetId={permissionSetId}&permissionSetToken={projectId}");
        }

        public static IVstsRestRequest<Response.PermissionsSetId> PermissionsGroupSetIdDefinition(string projectId,
            string permissionSetId, string applicationGroupId, string definitionId)
        {
            return new VstsRestRequest<PermissionsSetId>(
                $"{projectId}/_api/_security/DisplayPermissions?__v=5&tfid={applicationGroupId}&permissionSetId={permissionSetId}&permissionSetToken={projectId}%2F{definitionId}");

        }
            
    }
    
}