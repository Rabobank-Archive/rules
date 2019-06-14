using System.Collections.Generic;
using SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.VstsService.Requests
{
    public static class Policies
    {
        public static IVstsRequest<Multiple<RequiredReviewersPolicy>> RequiredReviewersPolicies(string project) => new VstsRequest<Multiple<RequiredReviewersPolicy>>($"{project}/_apis/policy/configurations", new Dictionary<string, string> 
            {
                { "policyType", "fd2167ab-b0be-447a-8ec8-39368250530e" }
            });

        public static IVstsRequest<Multiple<MinimumNumberOfReviewersPolicy>> MinimumNumberOfReviewersPolicies(string project) => new VstsRequest<Multiple<MinimumNumberOfReviewersPolicy>>($"{project}/_apis/policy/configurations", new Dictionary<string, string> 
            {
                { "policyType", "fa4e907d-c16b-4a4c-9dfa-4906e5d171dd" }
                
            });

        public static IVstsRequest<Multiple<Policy>> All(string project) => new VstsRequest<Multiple<Policy>>($"{project}/_apis/policy/configurations", new Dictionary<string, string> 
            {
                { "api-version", "5.0-preview.1" }
            });

        public static IVstsRequest<Policy> Policy(string project, int id) => new VstsRequest<Policy>($"{project}/_apis/policy/configurations/{id}", new Dictionary<string, string> 
        {
            { "api-version", "5.0" }
        });
        
        public static IVstsRequest<Policy, Policy> Policy(string project) => new VstsRequest<Policy, Policy>($"{project}/_apis/policy/configurations", new Dictionary<string, string> 
        {
            { "api-version", "5.0" }
        });
    }
}