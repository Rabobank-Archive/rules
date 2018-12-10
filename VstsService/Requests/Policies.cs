using RestSharp;

namespace SecurePipelineScan.VstsService.Requests
{
    public static class Policies
    {
        public static IVstsRestRequest<Response.Multiple<Response.RequiredReviewersPolicy>> RequiredReviewersPolicies(string project)
        {
            return new VstsRestRequest<Response.Multiple<Response.RequiredReviewersPolicy>>($"{project}/_apis/policy/configurations?policyType=fd2167ab-b0be-447a-8ec8-39368250530e");
        }

        public static IVstsRestRequest<Response.Multiple<Response.MinimumNumberOfReviewersPolicy>> MinimumNumberOfReviewersPolicies(string project)
        {
            return new VstsRestRequest<Response.Multiple<Response.MinimumNumberOfReviewersPolicy>>($"{project}/_apis/policy/configurations?policyType=fa4e907d-c16b-4a4c-9dfa-4906e5d171dd");
        }
    }
}