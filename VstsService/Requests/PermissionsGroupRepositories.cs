using RestSharp;
using RestSharp.Extensions;
using SecurePipelineScan.VstsService.Response;
using System.Collections;

namespace SecurePipelineScan.VstsService.Requests
{
    public static class PermissionsGroupRepositories
    {
        public static IVstsRestRequest<Response.PermissionsRepository> PermissionsGroupRepositorySet(string projectId, string permissionSetId, string applicationGroupId)
        {
            return new VstsRestRequest<Response.PermissionsRepository>(
                $"{projectId}/_api/_security/DisplayPermissions?__v=5&tfid={applicationGroupId}&permissionSetId={permissionSetId}&permissionSetToken=repoV2%2F{projectId}");
        }
        
        public static IVstsRestRequest<Response.PermissionsRepository> PermissionsGroupRepository(string projectId, string permissionSetId, string applicationGroupId, string repositoryId)
        {
            return new VstsRestRequest<PermissionsRepository>(
                $"{projectId}/_api/_security/DisplayPermissions?__v=5&tfid={applicationGroupId}&permissionSetId={permissionSetId}&permissionSetToken=repoV2%2F{projectId}%2F{repositoryId}");
        }
    }
    
}