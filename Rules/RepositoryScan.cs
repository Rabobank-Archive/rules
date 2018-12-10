using Rules.Reports;
using SecurePipelineScan.VstsService;
using System.Collections.Generic;
using SecurePipelineScan.Rules.Checks;
using Requests = SecurePipelineScan.VstsService.Requests;

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
            var repos = client.Get(Requests.Repository.Repositories(project)).Value;
            var minimumNumberOfReviewersPolicies = client.Get(Requests.Policies.MinimumNumberOfReviewersPolicies(project));

            foreach (var repo in repos)
            {
                var repoReport = new RepositoryReport
                {
                    Project = project,
                    Repository = repo.Name,
                    HasRequiredReviewerPolicy = repo.HasRequiredReviewerPolicy(minimumNumberOfReviewersPolicies.Value)
                };

                yield return repoReport;
            }
        }
    }
}