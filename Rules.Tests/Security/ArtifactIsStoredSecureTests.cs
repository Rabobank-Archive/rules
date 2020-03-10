using System;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Newtonsoft.Json.Linq;
using NSubstitute;
using SecurePipelineScan.Rules.Security;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Response;
using Shouldly;
using VstsService.Response;
using Xunit;
using static SecurePipelineScan.VstsService.Requests.YamlPipeline;
using Task = System.Threading.Tasks.Task;

namespace SecurePipelineScan.Rules.Tests.Security
{
    public class ArtifactIsStoredSecureTests
    {
        private readonly Fixture _fixture = new Fixture { RepeatCount = 1 };

        public ArtifactIsStoredSecureTests()
        {
            _fixture.Customize(new AutoNSubstituteCustomization());
        }

        [Fact]
        public async Task GivenPipeline_WhenYamlWithPublish_ThenEvaluatesToTrue()
        {
            _fixture.Customize<BuildProcess>(ctx => ctx
                .With(p => p.Type, 2));
            _fixture.Customize<Project>(ctx => ctx
                .With(x => x.Name, "projectA"));
            _fixture.Customize<Repository>(ctx => ctx
                .With(r => r.Url, new Uri("https://projectA.nl")));

            var buildPipeline = _fixture.Create<BuildDefinition>();
            var project = _fixture.Create<Project>();

            var yamlResponse = new YamlPipelineResponse { FinalYaml = "steps:\r- task: ecdc45f6-832d-4ad9-b52b-ee49e94659be@1" };

            var client = Substitute.For<IVstsRestClient>();
            client.PostAsync(Arg.Any<IVstsRequest<YamlPipelineRequest, YamlPipelineResponse>>(), Arg.Any<YamlPipelineRequest>()).Returns(yamlResponse);

            var rule = new ArtifactIsStoredSecure(client);
            var result = await rule.EvaluateAsync(project, buildPipeline);

            result.ShouldBe(true);
        }

        [Fact]
        public async Task GivenPipeline_WhenYamlFileWithJobsAndPublish_ThenEvaluatesToTrue()
        {
            _fixture.Customize<BuildProcess>(ctx => ctx
                .With(p => p.Type, 2));
            _fixture.Customize<Project>(ctx => ctx
                .With(x => x.Name, "projectA"));
            _fixture.Customize<Repository>(ctx => ctx
                .With(r => r.Url, new Uri("https://projectA.nl")));

            var buildPipeline = _fixture.Create<BuildDefinition>();
            var project = _fixture.Create<Project>();

            var yamlResponse = new YamlPipelineResponse { FinalYaml = "jobs:\n- steps:\n  - task: PublishBuildArtifacts@2\n\n" };
            var client = Substitute.For<IVstsRestClient>();
            client.PostAsync(Arg.Any<IVstsRequest<YamlPipelineRequest, YamlPipelineResponse>>(), Arg.Any<YamlPipelineRequest>()).Returns(yamlResponse);

            var rule = new ArtifactIsStoredSecure(client);
            var result = await rule.EvaluateAsync(project, buildPipeline);

            result.ShouldBe(true);
        }

        [Fact]
        public async Task GivenPipeline_WhenYamlFileWithStagesAndPublish_ThenEvaluatesToTrue()
        {
            _fixture.Customize<BuildProcess>(ctx => ctx
                .With(p => p.Type, 2));
            _fixture.Customize<Project>(ctx => ctx
                .With(x => x.Name, "projectA"));
            _fixture.Customize<Repository>(ctx => ctx
                .With(r => r.Url, new Uri("https://projectA.nl")));

            var buildPipeline = _fixture.Create<BuildDefinition>();
            var project = _fixture.Create<Project>();

            var yamlResponse = new YamlPipelineResponse { FinalYaml = "stages:\n- jobs:\n  - steps:\n    - task: PublishBuildArtifacts@2\n\n" };
            var client = Substitute.For<IVstsRestClient>();
            client.PostAsync(Arg.Any<IVstsRequest<YamlPipelineRequest, YamlPipelineResponse>>(), Arg.Any<YamlPipelineRequest>()).Returns(yamlResponse);

            var rule = new ArtifactIsStoredSecure(client);
            var result = await rule.EvaluateAsync(project, buildPipeline);

            result.ShouldBe(true);
        }

        [Fact]
        public async Task GivenPipeline_WhenYamlFileWithoutContent_ThenEvaluatesToFalse()
        {
            _fixture.Customize<BuildProcess>(ctx => ctx
                .With(p => p.Type, 2));
            _fixture.Customize<Project>(ctx => ctx
                .With(x => x.Name, "projectA"));
            _fixture.Customize<Repository>(ctx => ctx
                .With(r => r.Url, new Uri("https://projectA.nl")));

            var buildPipeline = _fixture.Create<BuildDefinition>();
            var project = _fixture.Create<Project>();

            var yamlResponse = new YamlPipelineResponse { };
            var client = Substitute.For<IVstsRestClient>();
            client.PostAsync(Arg.Any<IVstsRequest<YamlPipelineRequest, YamlPipelineResponse>>(), Arg.Any<YamlPipelineRequest>()).Returns(yamlResponse);

            var rule = new ArtifactIsStoredSecure(client);
            var result = await rule.EvaluateAsync(project, buildPipeline);

            result.ShouldBe(false);
        }

        [Fact]
        public async Task GivenPipeline_WhenCorruptYamlFile_ThenEvaluatesToFalse()
        {
            _fixture.Customize<BuildProcess>(ctx => ctx
                .With(p => p.Type, 2));
            _fixture.Customize<Project>(ctx => ctx
                .With(x => x.Name, "projectA"));
            _fixture.Customize<Repository>(ctx => ctx
                .With(r => r.Url, new Uri("https://projectA.nl")));


            var buildPipeline = _fixture.Create<BuildDefinition>();
            var project = _fixture.Create<Project>();

            var yamlResponse = new YamlPipelineResponse { FinalYaml = "invalid" };
            var client = Substitute.For<IVstsRestClient>();
            client.PostAsync(Arg.Any<IVstsRequest<YamlPipelineRequest, YamlPipelineResponse>>(), Arg.Any<YamlPipelineRequest>()).Returns(yamlResponse);

            var rule = new ArtifactIsStoredSecure(client);
            var result = await rule.EvaluateAsync(project, buildPipeline);

            result.ShouldBe(false);
        }

        [Fact]
        public async Task GivenPipeline_WhenYamlFileWithDisabledPublish_ThenEvaluatesToFalse()
        {
            _fixture.Customize<BuildProcess>(ctx => ctx
                .With(p => p.Type, 2));
            _fixture.Customize<Project>(ctx => ctx
                .With(x => x.Name, "project A"));
            _fixture.Customize<Repository>(ctx => ctx
                .With(r => r.Url, new Uri("https://urlwithotherproject.nl")));

            var buildPipeline = _fixture.Create<BuildDefinition>();
            var project = _fixture.Create<Project>();

            var yamlResponse = new YamlPipelineResponse { FinalYaml = "steps:\r- task: PublishBuildArtifacts@1\r enabled: false" };
            var client = Substitute.For<IVstsRestClient>();
            client.PostAsync(Arg.Any<IVstsRequest<YamlPipelineRequest, YamlPipelineResponse>>(), Arg.Any<YamlPipelineRequest>()).Returns(yamlResponse);

            var rule = new ArtifactIsStoredSecure(client);
            var result = await rule.EvaluateAsync(project, buildPipeline);

            result.ShouldBe(false);
        }
    }
}