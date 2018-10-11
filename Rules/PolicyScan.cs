using Rules.Reports;
using SecurePipelineScan.Rules.Reports;
using SecurePipelineScan.VstsService;
using System;
using System.Collections.Generic;
using Response = SecurePipelineScan.VstsService.Response;
using SecurePipelineScan.Rules.Checks;

namespace SecurePipelineScan.Rules
{
    public class PolicyScan : IScan
    {
        private readonly IVstsRestClient client;
        private readonly Action<IEnumerable<ScanReport>> progress;

        public PolicyScan(IVstsRestClient client, Action<IEnumerable<ScanReport>> progress)
        {
            this.client = client;
            this.progress = progress;
        }

        public void Execute(string project)
        {
            var repos = client.Execute(Requests.Repository.Repositories(project)).Data.Value;
            var minimumNumberOfReviewersPolicies = client.Execute(Requests.Policies.MinimumNumberOfReviewersPolicies(project));



            List<BranchPolicyReport> report = new List<BranchPolicyReport>(); 

            foreach (var repo in repos)
            {
                var repoReport = new BranchPolicyReport();
                repoReport.Project = project;
                repoReport.Repository = repo.Name;
                repoReport.HasRequiredReviewerPolicy = repo.HasRequiredReviewerPolicy(minimumNumberOfReviewersPolicies.Data.Value);

                report.Add(repoReport);
            }

            progress(report);
        }
    }
}