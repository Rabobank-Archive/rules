﻿using AutoFixture;
using AutoFixture.AutoNSubstitute;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Requests;
using Xunit;
using Xunit.Abstractions;
using static SecurePipelineScan.VstsService.Requests.ExtensionManagement;

namespace SecurePipelineScan.Rules.Tests.IntegrationTests
{
    public class ExtensionManagementProjectOverview : IClassFixture<TestConfig>
    {
        private readonly ITestOutputHelper _output;
        private readonly TestConfig _config;
        private readonly VstsRestClient _client;

        public ExtensionManagementProjectOverview(ITestOutputHelper output, TestConfig config)
        {
            _output = output;
            _config = config;
            _client = new VstsRestClient(config.Organization, config.Token);
        }

        [Fact]
        [Trait("category", "integration")]
        public void PutShouldInsertDemoDataProjectOverview()
        {
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());

            fixture.Customizations.Add(
                new RandomNumericSequenceGenerator(0, 5));

            var data = fixture.Create<ProjectOverviewData>();

            data.Id = "SOx-compliant-demo";
            data.Etag = -1;

            string extensionName = _config.ExtensionName;

            _client.Put(ExtensionData<ProjectOverviewData>("tas", extensionName,
                "ProjectOverview"), data);
        }
    }
}