using Rules.Reports;
using SecurePipelineScan.Rules.Reports;
using SecurePipelineScan.VstsService;
using System;
using System.Collections.Generic;
using Response = SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.Rules
{
    public class PolicyScan : IScan
    {
        private readonly IVstsRestClient client;
        private readonly Action<ScanReport> progress;

        public PolicyScan(IVstsRestClient client, Action<ScanReport> progress)
        {
            this.client = client;
            this.progress = progress;
        }

        public void Execute(string project)
        {
            var branchPolicies = client.Execute(Requests.Policies.MinimumNumberOfReviewersPolicies(project));

            Execute(project, branchPolicies.ThrowOnError().Data.Value);
        }

        private void Execute(string project, IEnumerable<Response.MinimumNumberOfReviewersPolicy> policies)
        {
            foreach (var policy in policies)
            {
                PrintPolicy(project, policy);
            }
        }

        private void PrintPolicy(string project, Response.MinimumNumberOfReviewersPolicy policy)
        {
            progress(new BranchPolicyReport()
            {
                BranchPolicy = policy,
            });
        }
    }
}