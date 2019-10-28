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
            var gitItem = await _client.GetAsync(Requests.Repository.GitItem("TAS",
                "c6bd6cf0-b7f6-40c4-8869-cadaa8f8718c", "/azure-pipelines.yml")
                .AsJson());

            var yamlText = gitItem.SelectToken("content").ToString();
            var yamlObject = new Deserializer().Deserialize(new StringReader(yamlText));

            var jsonText = JsonConvert.SerializeObject(yamlObject);
            var jsonObject = JsonConvert.DeserializeObject<JObject>(jsonText);

            var containsTask = jsonObject.SelectTokens("steps[*]")
                .Any(s => s.SelectToken("task", false)?.ToString() == "PublishBuildArtifacts@1"
                    && s.SelectToken("enabled", false)?.ToString() != "false");

            containsTask.ShouldBeTrue();
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
