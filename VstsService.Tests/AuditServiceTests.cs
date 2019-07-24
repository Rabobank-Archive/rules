using System;
using System.Linq;
using ExpectedObjects;
using SecurePipelineScan.VstsService.Requests;
using Shouldly;
using Xunit;

namespace SecurePipelineScan.VstsService.Tests
{
    public class AuditServiceTests : IClassFixture<TestConfig>
    {
        private readonly VstsRestClient _client;

        public AuditServiceTests(TestConfig config)
        {
            _client = new VstsRestClient(config.Organization, config.Token);
        }
        
        [Fact]
        public void TestQueryContinuation()
        {
            _client.Get(AuditLog.Query()).Take(300).Count().ShouldBe(300);
        }
        
        [Fact]
        public void TestAuditLogEntry()
        {
            var expected = new
            {
                ActionId = Expect.NotDefault<string>(),
                Details = Expect.NotDefault<string>(),
                Area = Expect.NotDefault<string>(),
                Category = Expect.NotDefault<string>(),
                ProjectId = Expect.NotDefault<string>(),
                Timestamp = Expect.NotDefault<DateTime>()
            }.ToExpectedObject();
            
            var result = _client.Get(AuditLog.Query()).Take(300);
            result.ShouldContain(e => expected.Matches(e));
        }
        
        [Fact]
        public void TestAuditLogEntryUsingStartAndEndDate()
        {
            var yesterday = DateTime.UtcNow.Date.AddDays(-1);
            var today = DateTime.UtcNow.Date;
            
            var result = _client.Get(AuditLog.Query(yesterday, today));
            result.ToList().ForEach(e => e.Timestamp.ShouldBeInRange(yesterday, today));
        }
    }
}