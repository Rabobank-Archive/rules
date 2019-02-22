using System;
using System.IO;
using AutoFixture;
using Newtonsoft.Json.Linq;
using NSubstitute;
using SecurePipelineScan.Rules.Events;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Requests;
using SecurePipelineScan.VstsService.Response;
using Shouldly;
using Xunit;
using Project = SecurePipelineScan.VstsService.Response.Project;

namespace SecurePipelineScan.Rules.Tests
{
    public class BuildScanTests : IClassFixture<TestConfig>
    {
        private readonly IFixture _fixture = new Fixture();
        
        [Fact]
        public void CompletedIncludesBuildDetailsLikeProjecName()
        {
            _fixture
                .Customize<Project>(x => x.With(p => p.Name, "LQA"));
            
            var input = ReadInput("Completed.json");
            var client = Substitute.For<IVstsRestClient>();
            client
                .Get(Arg.Any<IVstsRestRequest<Build>>())
                .Returns(_fixture.Create<Build>());

            client
                .Get(Arg.Any<IVstsRestRequest<Multiple<BuildArtifact>>>())
                .Returns(_fixture.Create<Multiple<BuildArtifact>>());

            client
                .Get(Arg.Any<IVstsRestRequest<JObject>>())
                .Returns(_fixture.Create<JObject>());

            var scan = new BuildScan(client);
            var report = scan.Completed(input);

            report.Id.ShouldBe("70609");
            report.Project.ShouldBe("LQA");   
            report.Pipeline.ShouldNotBeEmpty();
            report.CreatedDate.ShouldNotBe(default(DateTime));
        }

        [Fact]
        public void CompletedIncludesTasksShouldAllBeFalse()
        {
            _fixture
                .Customize<Project>(x => x.With(p => p.Name, "LQA"));

            var input = ReadInput("Completed.json");
            var timeline = ReadInput("DesignerBuildTimeline.json");

            var client = Substitute.For<IVstsRestClient>();
            client
                .Get(Arg.Any<IVstsRestRequest<Build>>())
                .Returns(_fixture.Create<Build>());

            client
                .Get(Arg.Any<IVstsRestRequest<Multiple<BuildArtifact>>>())
                .Returns(_fixture.Create<Multiple<BuildArtifact>>());

            client
                .Get(Arg.Any<IVstsRestRequest<JObject>>())
                .Returns(timeline);

            var scan = new BuildScan(client);
            var report = scan.Completed(input);

            report.UsesFortify.ShouldBe(false);
            report.UsesSonarQube.ShouldBe(false);

        }
        [Fact]
        public void CompletedIncludesTasksShouldAllBeTrue()
        {
            _fixture
                .Customize<Project>(x => x.With(p => p.Name, "LQA"));

            var input = ReadInput("Completed.json");
            var timeline = ReadInput("YamlBuildTimeline.json");

            var client = Substitute.For<IVstsRestClient>();
            client
                .Get(Arg.Any<IVstsRestRequest<Build>>())
                .Returns(_fixture.Create<Build>());

            client
                .Get(Arg.Any<IVstsRestRequest<Multiple<BuildArtifact>>>())
                .Returns(_fixture.Create<Multiple<BuildArtifact>>());

            client
                .Get(Arg.Any<IVstsRestRequest<JObject>>())
                .Returns(timeline);

            var scan = new BuildScan(client);
            var report = scan.Completed(input);

            report.UsesFortify.ShouldBe(true);
            report.UsesSonarQube.ShouldBe(true);
        }


        [Fact]
        public void AllArtifactsInContainer_ArtifactsStoredSecure_ShouldBeTrue()
        {
            _fixture.Customize<ArtifactResource>(x =>
                x.With(a => a.Type, "Container"));
            
            var input = ReadInput("Completed.json");
            var client = Substitute.For<IVstsRestClient>();
            client
                .Get(Arg.Any<IVstsRestRequest<Build>>())
                .Returns(_fixture.Create<Build>());
            
            client
                .Get(Arg.Any<IVstsRestRequest<Multiple<BuildArtifact>>>())
                .Returns(_fixture.Create<Multiple<BuildArtifact>>());

            client
                .Get(Arg.Any<IVstsRestRequest<JObject>>())
                .Returns(_fixture.Create<JObject>());


            var scan = new BuildScan(client);
            var report = scan.Completed(input);

            report
                .ArtifactsStoredSecure
                .ShouldBeTrue();
        }
        
        [Fact]
        public void NotAllArtifactsInContainer_ArtifactsStoredSecure_ShouldBeFalse()
        {
            var input = ReadInput("Completed.json");
            var client = Substitute.For<IVstsRestClient>();
            client
                .Get(Arg.Any<IVstsRestRequest<Build>>())
                .Returns(_fixture.Create<Build>());
            
            client
                .Get(Arg.Any<IVstsRestRequest<Multiple<BuildArtifact>>>())
                .Returns(_fixture.Create<Multiple<BuildArtifact>>());

            client
                .Get(Arg.Any<IVstsRestRequest<JObject>>())
                .Returns(_fixture.Create<JObject>());


            var scan = new BuildScan(client);
            var report = scan.Completed(input);

            report
                .ArtifactsStoredSecure
                .ShouldBeFalse();
        }

        private static JObject ReadInput(string eventType)
        {
            var path = Path.Join("Assets", "Build", eventType);
            return JObject.Parse(File.ReadAllText(path));
        }
    }
}