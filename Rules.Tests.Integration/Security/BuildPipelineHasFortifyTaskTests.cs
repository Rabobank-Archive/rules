using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using SecurePipelineScan.Rules.Security;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Requests;
using Shouldly;
using Xunit;

namespace Rules.Tests.Integration.Security
{
    public class BuildPipelineHasFortifyTaskTests : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;
        private readonly Fixture _fixture = new Fixture { RepeatCount = 1 };
        private const string TaskName = "FortifySCA";

        public BuildPipelineHasFortifyTaskTests(TestConfig config)
        {
            _config = config;
            _fixture.Customize(new AutoNSubstituteCustomization());
        }

        #region [ gui ]

        [Fact]
        [Trait("category", "integration")]
        public async Task EvaluateGuiIntegrationTest()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var project = await client.GetAsync(Project.ProjectById(_config.Project));
            var buildPipeline = await client.GetAsync(Builds.BuildDefinition(project.Id, "2"))
                .ConfigureAwait(false);

            var rule = new BuildPipelineHasFortifyTask(client);
            (await rule.EvaluateAsync(project, buildPipeline).ConfigureAwait(false))
                .GetValueOrDefault().ShouldBeTrue();
        }

        #endregion

        #region [ yaml ]

        [Fact]
        [Trait("category", "integration")]
        public async Task EvaluateYamlIntegrationTest()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var project = await client.GetAsync(Project.ProjectById(_config.Project));
            var buildPipeline = await client.GetAsync(Builds.BuildDefinition(project.Id, "197"))
                .ConfigureAwait(false);

            var rule = new BuildPipelineHasFortifyTask(client);
            (await rule.EvaluateAsync(project, buildPipeline).ConfigureAwait(false))
                .GetValueOrDefault().ShouldBeFalse();
        }

        [Fact]
        [Trait("category", "integration")]
        public async Task EvaluateNestedYamlTemplatesIntegrationTest()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var project = await client.GetAsync(Project.ProjectById(_config.Project));
            var buildPipeline = await client.GetAsync(Builds.BuildDefinition(project.Id, "275"))
                .ConfigureAwait(false);

            var rule = new BuildPipelineHasFortifyTask(client);
            (await rule.EvaluateAsync(project, buildPipeline).ConfigureAwait(false))
                .GetValueOrDefault().ShouldBeTrue();
        }

        #endregion
    }
}