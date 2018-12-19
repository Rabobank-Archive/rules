using RestSharp;
using RestSharp.Extensions;
using SecurePipelineScan.VstsService.Response;
using System.Collections;

namespace SecurePipelineScan.VstsService.Requests
{
    public static class PermissionsGroupRepoSet
    {
        public static IVstsRestRequest<Response.PermissionsGitRepositorySet> PermissionsGitRepositorySet(string projectId, string permissionSetId, string applicationGroupId)
        {
            return new VstsRestRequest<Response.PermissionsGitRepositorySet>(
                $"{projectId}/_api/_security/DisplayPermissions?__v=5&tfid={applicationGroupId}&permissionSetId={permissionSetId}&permissionSetToken=repoV2%2F{projectId}");
        }
    }
}