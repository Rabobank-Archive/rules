using Rules.Reports;
using SecurePipelineScan.Rules.Reports;
using SecurePipelineScan.VstsService;
using System;
using System.Collections.Generic;
using Response = SecurePipelineScan.VstsService.Response;
using SecurePipelineScan.Rules.Checks;

namespace SecurePipelineScan.Rules
{
    public class RepositoryScan
    {
        private readonly IVstsRestClient client;

        public RepositoryScan(IVstsRestClient client)
        {
            this.client = client;
        }

        public IEnumerable<RepositoryReport> Execute(string project)
        {
            var repos = client.Execute(Requests.Repository.Repositories(project)).Data.Value;
            var minimumNumberOfReviewersPolicies = client.Execute(Requests.Policies.MinimumNumberOfReviewersPolicies(project));

            foreach (var repo in repos)
            {
                var repoReport = new RepositoryReport();
                repoReport.Project = project;
                repoReport.Repository = repo.Name;
                repoReport.HasRequiredReviewerPolicy = repo.HasRequiredReviewerPolicy(minimumNumberOfReviewersPolicies.Data.Value);

                yield return repoReport;
            }
        }
    }
}