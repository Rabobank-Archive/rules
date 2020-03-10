using System;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using SecurePipelineScan.Rules.Security;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Requests;
using Shouldly;
using Xunit;
using Project = SecurePipelineScan.VstsService.Requests.Project;
using Response = SecurePipelineScan.VstsService.Response;
using Task = System.Threading.Tasks.Task;

namespace Rules.Tests.Integration.Security
{
    public class ArtifactIsStoredSecureTests : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;
        private readonly Fixture _fixture = new Fixture { RepeatCount = 1 };
        private readonly IVstsRestClient _client;

        private const string MavenTaskId = "ac4ee482-65da-4485-a532-7b085873e532";

        public ArtifactIsStoredSecureTests(TestConfig config)
        {
            _config = config;
            _fixture.Customize(new AutoNSubstituteCustomization());
            _client = new VstsRestClient(_config.Organization, _config.Token);
        }

        [Fact]
        [Trait("category", "integration")]
        public async Task EvaluateBuildIntegrationTest_gui()
        {
            var project = (await _client.GetAsync(Project.ProjectById(_config.Project)));
            var buildPipeline = await _client.GetAsync(Builds.BuildDefinition(project.Id, "2"))
                .ConfigureAwait(false);

            var rule = new ArtifactIsStoredSecure(_client);
            var result = await rule.EvaluateAsync(project, buildPipeline);

            result.GetValueOrDefault().ShouldBeTrue();
        }

        [Fact]
        [Trait("category", "integration")]
        public async Task EvaluateBuildIntegrationTest_Yaml()
        {
            var project = await _client.GetAsync(Project.ProjectById(_config.Project));
            var buildPipeline = await _client.GetAsync(Builds.BuildDefinition(project.Id, "275"))
                .ConfigureAwait(false);

            var rule = new ArtifactIsStoredSecure(_client);
            var result = await rule.EvaluateAsync(project, buildPipeline);

            result.ShouldBe(true);
        }

        [Fact]
        [Trait("category", "integration")]
        public async Task EvaluateBuildIntegrationTest_InvalidYaml()
        {
            var project = await _client.GetAsync(Project.ProjectById(_config.Project));
            var buildPipeline = await _client.GetAsync(Builds.BuildDefinition(project.Id, "201"))
                .ConfigureAwait(false);

            var rule = new ArtifactIsStoredSecure(_client);
            var result = await rule.EvaluateAsync(project, buildPipeline);

            result.ShouldBe(false);
        }

        [Fact]
        [Trait("category", "integration")]
        public async Task GivenPipeline_WhenGuiAndNoPhases_ThenEvaluatesToException()
        {
            _fixture.Customize<Response.BuildProcess>(ctx => ctx
                .With(p => p.Type, 1)
                .Without(p => p.Phases));
            var buildPipeline = _fixture.Create<Response.BuildDefinition>();
            var project = _fixture.Create<Response.Project>();

            var rule = new ArtifactIsStoredSecure(_client);
            var exception = await Record.ExceptionAsync(async () =>
                await rule.EvaluateAsync(project, buildPipeline));

            exception.ShouldNotBeNull();
        }

        [Fact]
        [Trait("category", "integration")]
        public async Task GivenPipeline_WhenGuiAndMavenAndTaskIsNotFound_ThenEvaluatesToNull()
        {
            _fixture.Customize<Response.BuildProcess>(ctx => ctx
                .With(p => p.Type, 1));
            _fixture.Customize<Response.BuildStep>(ctx => ctx
                .With(s => s.Enabled, true));
            _fixture.Customize<Response.BuildTask>(ctx => ctx
                .With(t => t.Id, MavenTaskId));
            var buildPipeline = _fixture.Create<Response.BuildDefinition>();
            var project = _fixture.Create<Response.Project>();

            var rule = new ArtifactIsStoredSecure(_client);
            var result = await rule.EvaluateAsync(project, buildPipeline);

            result.ShouldBeNull();
        }
    }
}