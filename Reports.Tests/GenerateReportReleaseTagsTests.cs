using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Tests;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using Xunit;
using Requests = SecurePipelineScan.VstsService.Requests;

namespace SecurePipelineScan.Reports.Tests
{
    public class GenerateReportReleaseTagsTests : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;

        public GenerateReportReleaseTagsTests(TestConfig config)
        {
            _config = config;
        }

        [Fact]
        public void ListAllTags()
        {
            var client = new VstsRestClient("somecompany", _config.Token);
            var projects = client.Get(Requests.Project.Projects());
            var all = new ConcurrentBag<string[]>();

            foreach (var project in projects.AsParallel())
            {
                var releases = client.Get(Requests.ReleaseManagement.Releases(project.Name, "tags", "2018-07-01T00:00:00.000Z"));

                foreach (var release in releases)
                {
                    foreach (var tag in release.Tags)
                    {
                        if (tag.Contains("SM9"))
                        {
                            var row = new string[] {
                                project.Name,
                                release.ReleaseDefinition.Name,
                                release.Name,
                                tag
                            };
                            all.Add(row);
                        }
                    }
                }
            }

            using (var output = new StreamWriter("tags.csv"))
            {
                foreach (var row in all)
                {
                    output.WriteLine("{0};{1};{2};{3}", row);
                }
            }
        }
    }
}