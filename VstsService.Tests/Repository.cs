using System;
using System.Linq;
using Flurl.Http;
using Shouldly;
using Xunit;

namespace SecurePipelineScan.VstsService.Tests
{
    [Trait("category", "integration")]
    public class Repository: IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;
        private readonly IVstsRestClient _client;

        public Repository(TestConfig config)
        {
            _config = config;
            _client = new VstsRestClient(config.Organization, config.Token);
        }

        [Fact]
        public void QueryRepository()
        {
            var definition = _client.Get(Requests.Repository.Repositories(_config.Project)).ToList();
            definition.ShouldNotBeEmpty();
            definition.ShouldAllBe(e => !string.IsNullOrEmpty(e.Name));
            definition.ShouldAllBe(e => !string.IsNullOrEmpty(e.Id));
            definition.ShouldAllBe(e => !string.IsNullOrEmpty(e.Project.Id));
            definition.ShouldAllBe(e => !string.IsNullOrEmpty(e.Project.Name));
            definition.ShouldAllBe(e => !string.IsNullOrEmpty(e.DefaultBranch));
        }

        [Fact]
        public void QueryPushes()
        {
            // Arrange
            var repository = _client.Get(Requests.Repository.Repositories(_config.Project)).First();

            // Act
            var pushes = _client.Get(Requests.Repository.Pushes(_config.Project, repository.Id)).ToList();  

            // Assert
            pushes.ShouldNotBeEmpty();
            var push = pushes.First();
            push.PushId.ShouldNotBe(0);
            push.Date.ShouldNotBe(default);
        }

        [Fact]
        public void PushThrows404()
        {
            var ex = Should.Throw<FlurlHttpException>(() => _client.Get(Requests.Repository.Pushes(_config.Project, Guid.NewGuid().ToString())).ToList());
            ex.Message.ShouldContain("404");
        }
    }
}
