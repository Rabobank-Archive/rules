using Newtonsoft.Json;
using Rules.Reports;
using SecurePipelineScan.Rules.Checks;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Converters;
using SecurePipelineScan.VstsService.Response;
using System.Collections.Generic;
using System.Linq;
using Requests = SecurePipelineScan.VstsService.Requests;

namespace SecurePipelineScan.Rules
{
    public class RepositoryScan : IProjectScan<IEnumerable<RepositoryReport>>
    {
        private readonly IVstsRestClient client;

        public RepositoryScan(IVstsRestClient client)
        {
            this.client = client;
        }

        public IEnumerable<RepositoryReport> Execute(string project)
        {
            var repos = client.Get(Requests.Repository.Repositories(project));
            var policies = client.Get(Requests.Policies.All(project));
            var minimumNumberOfReviewersPolicies = policies.OfType<MinimumNumberOfReviewersPolicy>();

            foreach (var repo in repos)
            {
                var repoReport = new RepositoryReport
                {
                    Project = project,
                    Repository = repo.Name,
                    HasRequiredReviewerPolicy = repo.HasRequiredReviewerPolicy(minimumNumberOfReviewersPolicies),
                };

                yield return repoReport;
            }
        }
    }
}