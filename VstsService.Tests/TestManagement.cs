using AutoFixture;
using SecurePipelineScan.VstsService.Requests;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SecurePipelineScan.VstsService.Tests
{
  public class TestManagement : IClassFixture<TestConfig>
  {
    private readonly TestConfig _config;
    private readonly IVstsRestClient _client;

    public TestManagement(TestConfig config)
    {
      _config = config;
      _client = new VstsRestClient(config.Organization, config.Token);
    }

    [Fact]
    public void GetManualTestRuns_ShouldReturnResults()
    {
      // Arrange
      var maxdate = DateTime.UtcNow;
      var mindate = maxdate.AddDays(-1);

      // Act
      var result = _client.Get(Requests.TestManagement.QueryTestRuns(_config.Project, mindate, maxdate, false));

      // Assert
      result.ShouldNotBeNull();
      result.ShouldBeEmpty();
    }


  }
}
