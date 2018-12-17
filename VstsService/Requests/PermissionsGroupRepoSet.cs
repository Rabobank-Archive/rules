using RestSharp;
using RestSharp.Extensions;
using SecurePipelineScan.VstsService.Response;
using System.Collections;

namespace SecurePipelineScan.VstsService.Requests
{
    public static class PermissionsGroupRepoSet
    {
        public static IVstsRestRequest<Response.PermissionsGitRepositorySet> PermissionsGitRepositorySet(string project)
        {
            const string applicationGroup = "b36e760a-9ce7-4432-842a-3f80cc6e7174";
            const string nameSpaceId = "2e9eb7ed-3c0a-47d4-87c1-0ffdd275fd87";
            const string projectId = "5470b9fd-156e-4308-a82c-c92886b5a99b";

            return new VstsRestRequest<Response.PermissionsGitRepositorySet>(
                $"{projectId}/_api/_security/DisplayPermissions?__v=5&tfid={applicationGroup}&permissionSetId={nameSpaceId}&permissionSetToken=repoV2%2F{projectId}");
        }
    }
}