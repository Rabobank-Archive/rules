using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shouldly;
using Xunit;
using YamlDotNet.Serialization;

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
        public async Task GetGitItem()
        {
            var gitItem = await _client.GetAsync(Requests.Repository.GitItem(_config.Project,
                "6435e3f0-15b7-4302-814d-4ab586e61f8b", "/azure-pipelines.yml")
                .AsJson()).ConfigureAwait(false);

            gitItem.ShouldNotBeNull();
        }

        [Fact]
        public void PushThrowsFor404()
        {
            var repositoryId = Guid.NewGuid().ToString();
            var ex = Should.Throw<NotFoundException>(() => _client.Get(Requests.Repository.Pushes(_config.Project, repositoryId)).ToList());
            ex.Message.ShouldContain(repositoryId);
        }
    }
}
