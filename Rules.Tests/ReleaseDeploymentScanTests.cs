using System;
using System.IO;
using ExpectedObjects;
using Newtonsoft.Json.Linq;
using NSubstitute;
using SecurePipelineScan.Rules.Events;
using SecurePipelineScan.Rules.Reports;
using SecurePipelineScan.VstsService;
using Shouldly;
using Xunit;

namespace SecurePipelineScan.Rules.Tests
{
    public class ReleaseDeploymentScanTests
    {
        public class Completed : IClassFixture<TestConfig>
        {
            private readonly TestConfig _config;
            private VstsRestClient _client;

            public Completed(TestConfig config)
            {
                _config = config;
                _client = new VstsRestClient(config.Organization, config.Token);
            }
            
            [Fact]
            public void ApprovalNotRequired()
            {
                var input = ReadInput("Completed", "NotApproved.json");
                var scan = new ReleaseDeploymentScan(Substitute.For<IServiceEndpointValidator>());
                
                var report = scan.Completed(input);
                report.HasApprovalOptions.ShouldBeFalse();
            }

            [Fact]
            public void MinimumNumberOfApproversNotZero()
            {
                var input = ReadInput("Completed", "Approved.json");
                var scan = new ReleaseDeploymentScan(Substitute.For<IServiceEndpointValidator>());

                var report = scan.Completed(input);
                report.HasApprovalOptions.ShouldBeTrue();
            }

            [Fact]
            public void RequestedForCanBeApprover()
            {
                var input = ReadInput("Completed", "ReleaseCreatorCanBeApprover.json");
                var scan = new ReleaseDeploymentScan(Substitute.For<IServiceEndpointValidator>());

                var report = scan.Completed(input);
                report.HasApprovalOptions.ShouldBeFalse();
            }

            [Fact]
            public void ReportInformation()
            {
                var expected = new ReleaseDeploymentCompletedReport
                {
                    Project = "Fabrikam",
                    Release = "Release-1",
                    Environment = "Dev",
                    ReleaseId = "1",
                    CreatedDate = DateTime.Parse("2016-09-19T13:03:28.5345098")
                }.ToExpectedObject(ctx =>
                {
                    ctx.Ignore(x => x.HasApprovalOptions);
                    ctx.Ignore(x => x.UsesProductionEndpoints);
                });

                var input = ReadInput("Completed", "ReleaseCreatorCanBeApprover.json");
                var scan = new ReleaseDeploymentScan(Substitute.For<IServiceEndpointValidator>());

                var report = scan.Completed(input);
                expected.ShouldEqual(report);
            }

            [Fact]
            public void UsesServiceEndpointValidatorToScan()
            {
                var input = ReadInput("Completed", "ReleaseCreatorCanBeApprover.json");
                var endpoints = Substitute.For<IServiceEndpointValidator>();
                endpoints.IsProduction(Arg.Any<string>(), Arg.Any<Guid>()).Returns(true);
                
                var scan = new ReleaseDeploymentScan(endpoints);
                var report = scan.Completed(input);
                
                report.UsesProductionEndpoints.ShouldBeTrue();
                endpoints
                    .Received()
                    .IsProduction("Fabrikam", new Guid("b460b0f8-fe23-4dc2-a99c-fd8b0633fe1c"));
            }

            [Fact]
            public void HowToHandleDefaults()
            {
                var expected = new ReleaseDeploymentCompletedReport
                {
                    // All default null values and false for booleans is fine
                }.ToExpectedObject();
                
                var input = new JObject();
                var scan = new ReleaseDeploymentScan(Substitute.For<IServiceEndpointValidator>());

                var report = scan.Completed(input);
                expected.ShouldEqual(report);
            }
        }

        private static JObject ReadInput(string eventType, string file)
        {
            var dir = Path.Join("Assets", "ReleaseDeployment", eventType);
            return JObject.Parse(File.ReadAllText(Path.Join(dir, file)));
        }
    }
}