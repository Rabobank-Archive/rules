using System.Collections.Generic;
using SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.VstsService.Requests
{
    public static class Policies
    {
        public static IEnumerableRequest<RequiredReviewersPolicy> RequiredReviewersPolicies(string project) => 
            new VstsRequest<RequiredReviewersPolicy>($"{project}/_apis/policy/configurations", new Dictionary<string, object> 
            {
                { "policyType", "fd2167ab-b0be-447a-8ec8-39368250530e" }
            }).AsEnumerable();

        public static IEnumerableRequest<MinimumNumberOfReviewersPolicy> MinimumNumberOfReviewersPolicies(string project) =>
            new VstsRequest<MinimumNumberOfReviewersPolicy>($"{project}/_apis/policy/configurations", new Dictionary<string, object> 
            {
                { "policyType", "fa4e907d-c16b-4a4c-9dfa-4906e5d171dd" }
                
            }).AsEnumerable();

        public static IEnumerableRequest<Policy> All(string project) => 
            new VstsRequest<Policy>($"{project}/_apis/policy/configurations", new Dictionary<string, object> 
            {
                { "api-version", "5.0-preview.1" }
            }).AsEnumerable();

        public static IVstsRequest<Policy> Policy(string project, int id) => 
            new VstsRequest<Policy>($"{project}/_apis/policy/configurations/{id}", new Dictionary<string, object> 
        {
            { "api-version", "5.0" }
        });
        
        public static IVstsRequest<Policy, Policy> Policy(string project) => 
            new VstsRequest<Policy, Policy>($"{project}/_apis/policy/configurations", new Dictionary<string, object> 
        {
            { "api-version", "5.0" }
        });
    }
}