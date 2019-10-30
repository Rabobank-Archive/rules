using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using SecurePipelineScan.Rules.Security;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Requests;
using Response = SecurePipelineScan.VstsService.Response;
using Shouldly;
using Xunit;
using System;
using Newtonsoft.Json.Linq;
using NSubstitute;

namespace SecurePipelineScan.Rules.Tests.Security
{
    public class BuildPipelineHasFortifyTaskTests : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;
        private readonly Fixture _fixture = new Fixture { RepeatCount = 1 };

        public BuildPipelineHasFortifyTaskTests(TestConfig config)
        {
            _config = config;
            _fixture.Customize(new AutoNSubstituteCustomization());
        }

        [Fact]
        [Trait("category", "integration")]
        public async Task EvaluateIntegrationTest_gui()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var project = await client.GetAsync(Project.ProjectById(_config.Project));
            var buildPipeline = await client.GetAsync(Builds.BuildDefinition(project.Id, "2"))
                .ConfigureAwait(false);

            var rule = new BuildPipelineHasFortifyTask(client);
            (await rule.EvaluateAsync(project, buildPipeline)).GetValueOrDefault().ShouldBeTrue();
        }

        [Fact]
        [Trait("category", "integration")]
        public async Task EvaluateIntegrationTest_yaml()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var project = await client.GetAsync(Project.ProjectById(_config.Project));
            var buildPipeline = await client.GetAsync(Builds.BuildDefinition(project.Id, "197"))
                .ConfigureAwait(false);

            var rule = new BuildPipelineHasFortifyTask(client);
            (await rule.EvaluateAsync(project, buildPipeline)).GetValueOrDefault().ShouldBeFalse();
        }

        [Fact]
        public async Task GivenPipeline_WhenYamlFileWithFortifyTask_ThenEvaluatesToTrue()
        {
            _fixture.Customize<Response.BuildProcess>(ctx => ctx
                .With(p => p.Type, 2));
            _fixture.Customize<Response.Project>(ctx => ctx
                .With(x => x.Name, "projectA"));
            _fixture.Customize<Response.Repository>(ctx => ctx
                .With(r => r.Url, new Uri("https://projectA.nl")));

            var gitItem = new JObject
            {
                { "content", "steps:\r- task: FortifySCA@5" }
            };

            var buildPipeline = _fixture.Create<Response.BuildDefinition>();
            var project = _fixture.Create<Response.Project>();

            var client = Substitute.For<IVstsRestClient>();
            client.GetAsync(Arg.Any<IVstsRequest<JObject>>()).Returns(gitItem);

            var rule = new BuildPipelineHasFortifyTask(client);
            var result = await rule.EvaluateAsync(project, buildPipeline);

            result.ShouldBe(true);
        }

        [Fact]
        public async Task GivenPipeline_WhenYamlFileWithoutFortifyTask_ThenEvaluatesToFalse()
        {
            _fixture.Customize<Response.BuildProcess>(ctx => ctx
                .With(p => p.Type, 2));
            _fixture.Customize<Response.Project>(ctx => ctx
                .With(x => x.Name, "projectA"));
            _fixture.Customize<Response.Repository>(ctx => ctx
                .With(r => r.Url, new Uri("https://projectA.nl")));

            var gitItem = new JObject
            {
                { "content", "steps:\r- task: OtherTask" }
            };

            var buildPipeline = _fixture.Create<Response.BuildDefinition>();
            var project = _fixture.Create<Response.Project>();

            var client = Substitute.For<IVstsRestClient>();
            client.GetAsync(Arg.Any<IVstsRequest<JObject>>()).Returns(gitItem);

            var rule = new BuildPipelineHasFortifyTask(client);
            var result = await rule.EvaluateAsync(project, buildPipeline);

            result.ShouldBe(false);
        }
    }
}