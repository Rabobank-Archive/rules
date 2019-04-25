//using System;
//using AutoFixture;
//using AutoFixture.AutoNSubstitute;
//using NSubstitute;
//using SecurePipelineScan.VstsService;
//using Shouldly;
//using System.Linq;
//using Xunit;
//using Xunit.Abstractions;
//using Response = SecurePipelineScan.VstsService.Response;
//
//namespace SecurePipelineScan.Rules.Tests
//{
//    public class RepositoryScanTests : IClassFixture<TestConfig>
//    {
//        private readonly ITestOutputHelper _output;
//        private readonly TestConfig _config;
//
//        public RepositoryScanTests(ITestOutputHelper output, TestConfig config)
//        {
//            _output = output;
//            _config = config;
//        }
//
//        [Fact]
//        [Trait("category", "integration")]
//        public void IntegrationTestOnScan()
//        {
//            var client = new VstsRestClient(_config.Organization, _config.Token);
//            var scan = new RepositoryScan(client);
//            var result = scan.Execute(_config.Project, DateTime.Now).ToList();
//
//            result.ForEach(x => _output.WriteLine($"Repository: {x.Repository}, Result: {x.HasRequiredReviewerPolicy}"));
//            result.ShouldAllBe(r => r.HasRequiredReviewerPolicy);
//        }
//
//        [Fact]
//        public void ReportsProgress()
//        {
//            var fixture = new Fixture();
//            fixture.Customize(new AutoNSubstituteCustomization());
//
//            var policy = fixture.Create<Response.Multiple<Response.Policy>>();
//
//            var repos = fixture.Create<Response.Multiple<Response.Repository>>();
//
//            var client = Substitute.For<IVstsRestClient>();
//            client
//                .Get(Arg.Any<IVstsRestRequest<Response.Multiple<Response.Policy>>>())
//                .Returns(policy);
//
//            client
//                .Get(Arg.Any<IVstsRestRequest<Response.Multiple<Response.Repository>>>())
//                .Returns(repos);
//
//            var scan = new RepositoryScan(client);
//            scan.Execute("dummy", DateTime.Now).ShouldNotBeEmpty();
//        }
//    }
//}