using System.Threading.Tasks;
using SecurePipelineScan.Rules.Security;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Requests;
using Response = SecurePipelineScan.VstsService.Response;
using Shouldly;
using Xunit;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using System;
using NSubstitute;
using Newtonsoft.Json.Linq;

namespace SecurePipelineScan.Rules.Tests.Security
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
        public async Task EvaluateBuildIntegrationTest()
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
            var buildPipeline = await _client.GetAsync(Builds.BuildDefinition(project.Id, "197"))
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
        public async Task GivenPipeline_WhenYamlFileInOtherProject_ThenEvaluatesToFalse()
        {
            _fixture.Customize<Response.BuildProcess>(ctx => ctx
                .With(p => p.Type, 2));
            _fixture.Customize<Response.Project>(ctx => ctx
                .With(x => x.Name, "project A"));
            _fixture.Customize<Response.Repository>(ctx => ctx
                .With(r => r.Url, new Uri("https://urlwithotherproject.nl")));

            var buildPipeline = _fixture.Create<Response.BuildDefinition>();
            var project = _fixture.Create<Response.Project>();

            var rule = new ArtifactIsStoredSecure(_client);
            var result = await rule.EvaluateAsync(project, buildPipeline);

            result.ShouldBe(false);
        }

        [Fact]
        public async Task GivenPipeline_WhenYamlWithPublish_ThenEvaluatesToTrue()
        {
            _fixture.Customize<Response.BuildProcess>(ctx => ctx
                .With(p => p.Type, 2));
            _fixture.Customize<Response.Project>(ctx => ctx
                .With(x => x.Name, "projectA"));
            _fixture.Customize<Response.Repository>(ctx => ctx
                .With(r => r.Url, new Uri("https://projectA.nl")));

            var gitItem = new JObject
            {
                { "content", "steps:\r- publish: drop" }
            };

            var buildPipeline = _fixture.Create<Response.BuildDefinition>();
            var project = _fixture.Create<Response.Project>();

            var client = Substitute.For<IVstsRestClient>();
            client.GetAsync(Arg.Any<IVstsRequest<JObject>>()).Returns(gitItem);

            var rule = new ArtifactIsStoredSecure(client);
            var result = await rule.EvaluateAsync(project, buildPipeline);

            result.ShouldBe(true);
        }

        [Fact]
        public async Task GivenPipeline_WhenYamlFileWithJobsAndPublish_ThenEvaluatesToTrue()
        {
            _fixture.Customize<Response.BuildProcess>(ctx => ctx
                .With(p => p.Type, 2));
            _fixture.Customize<Response.Project>(ctx => ctx
                .With(x => x.Name, "projectA"));
            _fixture.Customize<Response.Repository>(ctx => ctx
                .With(r => r.Url, new Uri("https://projectA.nl")));

            var gitItem = new JObject
            {
                { "content", "jobs:\n- steps:\n  - task: PublishBuildArtifacts@2\n\n" }
            };

            var buildPipeline = _fixture.Create<Response.BuildDefinition>();
            var project = _fixture.Create<Response.Project>();

            var client = Substitute.For<IVstsRestClient>();
            client.GetAsync(Arg.Any<IVstsRequest<JObject>>()).Returns(gitItem);

            var rule = new ArtifactIsStoredSecure(client);
            var result = await rule.EvaluateAsync(project, buildPipeline);

            result.ShouldBe(true);
        }

        [Fact]
        public async Task GivenPipeline_WhenYamlFileWithStagesAndPublish_ThenEvaluatesToTrue()
        {
            _fixture.Customize<Response.BuildProcess>(ctx => ctx
                .With(p => p.Type, 2));
            _fixture.Customize<Response.Project>(ctx => ctx
                .With(x => x.Name, "projectA"));
            _fixture.Customize<Response.Repository>(ctx => ctx
                .With(r => r.Url, new Uri("https://projectA.nl")));

            var gitItem = new JObject
            {
                { "content", "stages:\n- jobs:\n  - steps:\n    - task: PublishBuildArtifacts@2\n\n" }
            };

            var buildPipeline = _fixture.Create<Response.BuildDefinition>();
            var project = _fixture.Create<Response.Project>();

            var client = Substitute.For<IVstsRestClient>();
            client.GetAsync(Arg.Any<IVstsRequest<JObject>>()).Returns(gitItem);

            var rule = new ArtifactIsStoredSecure(client);
            var result = await rule.EvaluateAsync(project, buildPipeline);

            result.ShouldBe(true);
        }

        [Fact]
        public async Task GivenPipeline_WhenYamlFileWithoutContent_ThenEvaluatesToFalse()
        {
            _fixture.Customize<Response.BuildProcess>(ctx => ctx
                .With(p => p.Type, 2));
            _fixture.Customize<Response.Project>(ctx => ctx
                .With(x => x.Name, "projectA"));
            _fixture.Customize<Response.Repository>(ctx => ctx
                .With(r => r.Url, new Uri("https://projectA.nl")));

            var gitItem = new JObject
            {
                { "something", "something" }
            };

            var buildPipeline = _fixture.Create<Response.BuildDefinition>();
            var project = _fixture.Create<Response.Project>();

            var client = Substitute.For<IVstsRestClient>();
            client.GetAsync(Arg.Any<IVstsRequest<JObject>>()).Returns(gitItem);

            var rule = new ArtifactIsStoredSecure(client);
            var result = await rule.EvaluateAsync(project, buildPipeline);

            result.ShouldBe(false);
        }

        [Fact]
        public async Task GivenPipeline_WhenCorruptYamlFile_ThenEvaluatesToFalse()
        {
            _fixture.Customize<Response.BuildProcess>(ctx => ctx
                .With(p => p.Type, 2));
            _fixture.Customize<Response.Project>(ctx => ctx
                .With(x => x.Name, "projectA"));
            _fixture.Customize<Response.Repository>(ctx => ctx
                .With(r => r.Url, new Uri("https://projectA.nl")));

            var gitItem = new JObject
            {
                { "content", "something" }
            };

            var buildPipeline = _fixture.Create<Response.BuildDefinition>();
            var project = _fixture.Create<Response.Project>();

            var client = Substitute.For<IVstsRestClient>();
            client.GetAsync(Arg.Any<IVstsRequest<JObject>>()).Returns(gitItem);

            var rule = new ArtifactIsStoredSecure(client);
            var result = await rule.EvaluateAsync(project, buildPipeline);

            result.ShouldBe(false);
        }

        [Fact]
        public async Task GivenPipeline_WhenYamlFileWithDisabledPublish_ThenEvaluatesToFalse()
        {
            _fixture.Customize<Response.BuildProcess>(ctx => ctx
                .With(p => p.Type, 2));
            _fixture.Customize<Response.Project>(ctx => ctx
                .With(x => x.Name, "project A"));
            _fixture.Customize<Response.Repository>(ctx => ctx
                .With(r => r.Url, new Uri("https://urlwithotherproject.nl")));

            var gitItem = new JObject
            {
                { "content", "steps:\r- task: PublishBuildArtifacts@1\r enabled: false" }
            };

            var buildPipeline = _fixture.Create<Response.BuildDefinition>();
            var project = _fixture.Create<Response.Project>();

            var client = Substitute.For<IVstsRestClient>();
            client.GetAsync(Arg.Any<IVstsRequest<JObject>>()).Returns(gitItem);

            var rule = new ArtifactIsStoredSecure(client);
            var result = await rule.EvaluateAsync(project, buildPipeline);

            result.ShouldBe(false);
        }

        [Fact]
        public async Task GivenPipeline_WhenNestedYaml_ThenEvaluatesToNull()
        {
            _fixture.Customize<Response.BuildProcess>(ctx => ctx
                .With(p => p.Type, 2));
            _fixture.Customize<Response.Project>(ctx => ctx
                .With(x => x.Name, "projectA"));
            _fixture.Customize<Response.Repository>(ctx => ctx
                .With(r => r.Url, new Uri("https://projectA.nl")));

            var gitItem = new JObject
            {
                { "content", "steps:\r- template: OtherYaml" }
            };

            var buildPipeline = _fixture.Create<Response.BuildDefinition>();
            var project = _fixture.Create<Response.Project>();

            var client = Substitute.For<IVstsRestClient>();
            client.GetAsync(Arg.Any<IVstsRequest<JObject>>()).Returns(gitItem);

            var rule = new ArtifactIsStoredSecure(client);
            var result = await rule.EvaluateAsync(project, buildPipeline);

            result.ShouldBeNull();
        }

        [Fact]
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