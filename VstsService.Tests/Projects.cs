using SecurePipelineScan.Rules;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Tests;
using Requests = SecurePipelineScan.VstsService.Requests;
using Shouldly;
using System.Diagnostics;
using System.Net;
using Xunit;

namespace VstsService.Tests
{
    [Trait("category", "integration")]
    public class Projects : IClassFixture<TestConfig>
    {
        private readonly TestConfig config;
        private readonly IVstsRestClient client;

        public Projects(TestConfig config)
        {
            this.config = config;
            client = new VstsRestClient(config.Organization, config.Token);
        }

        /// <summary>
        /// Test if all projects have a Name
        /// </summary>
        [Fact]
        public void QueryProjects()
        {
            var definitions = client.Execute(Requests.Project.Projects());

            definitions.StatusCode.ShouldBe(HttpStatusCode.OK);
            definitions.Data.Value.ShouldAllBe(_ => !string.IsNullOrEmpty(_.Name));
        }

        [Fact]
        public void GetAppReports()
        {
            var projects = client.Execute(Requests.Project.Projects()).Data;

            foreach (var p in projects.Value)
            {
                Debug.WriteLine($"{p.Name} {p.Description}");
                var scan = new RepositoryScan(client);

                var results = scan.Execute(p.Name);

                foreach (var r in results)
                {
                    Debug.WriteLine($"{r.Project} {r.Repository} {r.HasRequiredReviewerPolicy}");
                }
            }
        }
    }
}