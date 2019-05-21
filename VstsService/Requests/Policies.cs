using SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.VstsService.Requests
{
    public static class Policies
    {
        public static IVstsRestRequest<Multiple<RequiredReviewersPolicy>> RequiredReviewersPolicies(string project) => new VstsRestRequest<Multiple<RequiredReviewersPolicy>>($"{project}/_apis/policy/configurations?policyType=fd2167ab-b0be-447a-8ec8-39368250530e");

        public static IVstsRestRequest<Multiple<MinimumNumberOfReviewersPolicy>> MinimumNumberOfReviewersPolicies(string project) => new VstsRestRequest<Multiple<MinimumNumberOfReviewersPolicy>>($"{project}/_apis/policy/configurations?policyType=fa4e907d-c16b-4a4c-9dfa-4906e5d171dd");

        public static IVstsRestRequest<Multiple<Policy>> All(string project) => new VstsRestRequest<Multiple<Policy>>($"{project}/_apis/policy/configurations?api-version=5.0-preview.1");

        public static IVstsRestRequest<Policy> Policy(string project, int id) => new VstsRestRequest<Policy>($"{project}/_apis/policy/configurations/{id}?api-version=5.0");  
    }
}