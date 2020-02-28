using System;
using System.IO;
using System.Linq;
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
using Task = System.Threading.Tasks.Task;

namespace SecurePipelineScan.Rules.Tests.Events
{
    public class BuildScanTests
    {
        private readonly IFixture _fixture = new Fixture();
        
        [Fact]
        public async Task CompletedIncludesBuildDetailsLikeProjectName()
        {
            _fixture
                .Customize<Project>(x => x.With(p => p.Name, "LQA"));
            _fixture.Customize<Build>(x => x.With(b => b.Result, "succeeded"));
            
            var input = ReadInput("Completed.json");
            var client = Substitute.For<IVstsRestClient>();
            client
                .GetAsync<Build>(Arg.Any<Uri>())
                .Returns(_fixture.Create<Build>());

            client
                .Get(Arg.Any<IEnumerableRequest<BuildArtifact>>())
                .Returns(_fixture.CreateMany<BuildArtifact>());

            client
                .GetAsync(Arg.Any<IVstsRequest<JObject>>())
                .Returns(_fixture.Create<JObject>());

            var scan = new BuildScan(client);
            var report = await scan.GetCompletedReportAsync(input);

            report.Id.ShouldBe("70609");
            report.Project.ShouldBe("LQA");   
            report.Pipeline.ShouldNotBeEmpty();
            report.CreatedDate.ShouldNotBe(default(DateTime));
        }

        [Fact]
        public async Task CompletedIncludesTasksShouldAllBeFalse()
        {
            _fixture
                .Customize<Project>(x => x.With(p => p.Name, "LQA"));
            _fixture.Customize<Build>(x => x.With(b => b.Result, "succeeded"));

            var input = ReadInput("Completed.json");
            var timeline = ReadInput("DesignerBuildTimeline.json");

            var client = Substitute.For<IVstsRestClient>();
            client
                .GetAsync<Build>(Arg.Any<Uri>())
                .Returns(_fixture.Create<Build>());

            client
                .Get(Arg.Any<IEnumerableRequest<BuildArtifact>>())
                .Returns(_fixture.CreateMany<BuildArtifact>());

            client
                .GetAsync(Arg.Any<IVstsRequest<JObject>>())
                .Returns(timeline);

            var scan = new BuildScan(client);
            var report = await scan.GetCompletedReportAsync(input);

            report.UsesFortify.ShouldBe(false);
            report.UsesSonarQube.ShouldBe(false);

        }
        
        [Fact]
        public async Task CompletedIncludesTasksShouldAllBeTrue()
        {
            _fixture
                .Customize<Project>(x => x.With(p => p.Name, "LQA"));
            _fixture.Customize<Build>(x => x.With(b => b.Result, "succeeded"));

            var input = ReadInput("Completed.json");
            var timeline = ReadInput("YamlBuildTimeline.json");

            var client = Substitute.For<IVstsRestClient>();
            client
                .GetAsync<Build>(Arg.Any<Uri>())
                .Returns(_fixture.Create<Build>());

            client
                .Get(Arg.Any<IEnumerableRequest<BuildArtifact>>())
                .Returns(_fixture.CreateMany<BuildArtifact>());

            client
                .GetAsync(Arg.Any<IVstsRequest<JObject>>())
                .Returns(timeline);

            var scan = new BuildScan(client);
            var report = await scan.GetCompletedReportAsync(input);

            report.UsesFortify.ShouldBe(true);
            report.UsesSonarQube.ShouldBe(true);
        }

        [Fact]
        public async Task CompletedWithNoTimelineShouldBeNull()
        {
            _fixture
                .Customize<Project>(x => x.With(p => p.Name, "LQA"));
            _fixture.Customize<Build>(x => x.With(b => b.Result, "succeeded"));

            var input = ReadInput("Completed.json");

            var client = Substitute.For<IVstsRestClient>();
            client
                .GetAsync<Build>(Arg.Any<Uri>())
                .Returns(_fixture.Create<Build>());

            client
                .Get(Arg.Any<IEnumerableRequest<BuildArtifact>>())
                .Returns(_fixture.CreateMany<BuildArtifact>());

            client
                .GetAsync(Arg.Any<IVstsRequest<JObject>>())
                .Returns((JObject)null);

            var scan = new BuildScan(client);
            var report = await scan.GetCompletedReportAsync(input);

            report.UsesFortify.ShouldBeNull();
            report.UsesSonarQube.ShouldBeNull();
        }
        

        [Fact]
        public async Task AllArtifactsInContainer_ArtifactsStoredSecure_ShouldBeTrue()
        {
            _fixture.Customize<ArtifactResource>(x =>
                x.With(a => a.Type, "Container"));
            _fixture.Customize<Build>(x => x.With(b => b.Result, "succeeded"));
            
            var input = ReadInput("Completed.json");
            var client = Substitute.For<IVstsRestClient>();
            client
                .GetAsync<Build>(Arg.Any<Uri>())
                .Returns(_fixture.Create<Build>());
            
            client
                .Get(Arg.Any<IEnumerableRequest<BuildArtifact>>())
                .Returns(_fixture.CreateMany<BuildArtifact>());

            client
                .GetAsync(Arg.Any<IVstsRequest<JObject>>())
                .Returns(_fixture.Create<JObject>());


            var scan = new BuildScan(client);
            var report = await scan.GetCompletedReportAsync(input);

            report
                .ArtifactsStoredSecure
                .ShouldBeTrue();
        }

        [Fact]
        public async Task AllArtifactsInContainerOrGitRef_ArtifactsStoredSecure_ShouldBeTrue()
        {
            _fixture.Customize<ArtifactResource>(x =>
                x.With(a => a.Type, "Container"));
            _fixture.Customize<Build>(x => x.With(b => b.Result, "succeeded"));

            var input = ReadInput("Completed.json");
            var client = Substitute.For<IVstsRestClient>();
            client
                .GetAsync<Build>(Arg.Any<Uri>())
                .Returns(_fixture.Create<Build>());

            var buildArtifacts = _fixture.CreateMany<BuildArtifact>().ToList();
            buildArtifacts.First().Resource.Type = "GitRef";

            client
                .Get(Arg.Any<IEnumerableRequest<BuildArtifact>>())
                .Returns(buildArtifacts);

            client
                .GetAsync(Arg.Any<IVstsRequest<JObject>>())
                .Returns(_fixture.Create<JObject>());

            var scan = new BuildScan(client);
            var report = await scan.GetCompletedReportAsync(input);

            report
                .ArtifactsStoredSecure
                .ShouldBeTrue();
        }

        [Fact]
        public async Task NotAllArtifactsInContainerOrGitRef_ArtifactsStoredSecure_ShouldBeFalse()
        {
            _fixture.Customize<Build>(x => x.With(b => b.Result, "succeeded"));
            
            var input = ReadInput("Completed.json");
            var client = Substitute.For<IVstsRestClient>();
            client
                .GetAsync<Build>(Arg.Any<Uri>())
                .Returns(_fixture.Create<Build>());
            
            client
                .Get(Arg.Any<IEnumerableRequest<BuildArtifact>>())
                .Returns(_fixture.CreateMany<BuildArtifact>());

            client
                .GetAsync(Arg.Any<IVstsRequest<JObject>>())
                .Returns(_fixture.Create<JObject>());


            var scan = new BuildScan(client);
            var report = await scan.GetCompletedReportAsync(input);

            report
                .ArtifactsStoredSecure
                .ShouldBeFalse();
        }

        [Theory]
        [InlineData("failed")]
        [InlineData("canceled")]
        public async Task GivenBuildNotSucceeded_WhenAnalysing_ThenReportIsNull(string result)
        {
            _fixture.Customize<Build>(ctx => ctx.With(x => x.Result, result));
            
            var input = ReadInput("Completed.json");
            var client = Substitute.For<IVstsRestClient>();
            client
                .GetAsync<Build>(Arg.Any<Uri>())
                .Returns(_fixture.Create<Build>());
            
            var scan = new BuildScan(client);
            var report = await scan.GetCompletedReportAsync(input);

            report.ShouldBeNull();
        }

        private static JObject ReadInput(string eventType)
        {
            var path = Path.Join("Assets", "Build", eventType);
            return JObject.Parse(File.ReadAllText(path));
        }
    }
}