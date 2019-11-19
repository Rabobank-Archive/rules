using SecurePipelineScan.VstsService.Response;
using System.Linq;
using System.Threading.Tasks;
using System;
using SecurePipelineScan.VstsService;
using Newtonsoft.Json.Linq;
using YamlDotNet.Serialization;
using System.IO;
using Newtonsoft.Json;
using YamlDotNet.Core;

namespace SecurePipelineScan.Rules.Security
{

    public class YamlPipelineEvaluator : IPipelineEvaluator
    {
        readonly IVstsRestClient _client;

        public YamlPipelineEvaluator(IVstsRestClient client)
        {
            _client = client;
        }

        public async Task<bool?> EvaluateAsync(Project project, BuildDefinition buildPipeline, IPipelineHasTaskRule rule)
        {
            if (project == null)
                throw new ArgumentNullException(nameof(project));

            if (buildPipeline == null)
                throw new ArgumentNullException(nameof(buildPipeline));

            if (buildPipeline.Process.YamlFilename == null)
                throw new ArgumentOutOfRangeException(nameof(buildPipeline));

            if (!buildPipeline.Repository.Url.ToString().ToUpperInvariant()
                    .Contains(project.Name.ToUpperInvariant()))
                return false;

            if (rule == null)
                throw new ArgumentNullException(nameof(rule));

            var yamlPipeline = await GetPipelineAsync(project.Id, buildPipeline.Repository.Id,
                    buildPipeline.Process.YamlFilename)
                .ConfigureAwait(false);

            if (yamlPipeline == null)
                return false;

            return PipelineContainsTask(yamlPipeline, rule);
        }

        private async Task<JObject> GetPipelineAsync(string projectId, string repositoryId,
            string yamlFileName)
        {
            var gitItem = await _client.GetAsync(VstsService.Requests.Repository.GitItem(
                projectId, repositoryId, yamlFileName)
                .AsJson()).ConfigureAwait(false);
            if (gitItem == null)
                return null;

            var yamlContent = gitItem.SelectToken("content", false)?.ToString();
            if (yamlContent == null)
                return null;

            return ConvertYamlToJson(yamlContent);
        }

        private static JObject ConvertYamlToJson(string yamlText)
        {
            try
            {
                var deserializer = new DeserializerBuilder().Build();
                var yamlObject = deserializer.Deserialize(new StringReader(yamlText));

                var serializer = new SerializerBuilder()
                    .JsonCompatible()
                    .Build();

                var json = serializer.Serialize(yamlObject);

                return JsonConvert.DeserializeObject<JObject>(json);
            }
            catch (Exception ex) when (ex is YamlDotNet.Core.SyntaxErrorException || ex is InvalidCastException || ex is YamlException)
            {
                return null;
            }
        }

        private static bool? PipelineContainsTask(JToken yamlPipeline, IPipelineHasTaskRule rule)
        {
            var steps = yamlPipeline.SelectTokens("steps[*]");
            if (yamlPipeline["jobs"] != null)
                steps = yamlPipeline.SelectTokens("jobs[*].steps[*]");

            var result = steps
                .Any(s => (s[rule.StepName] != null ||
                    (s["task"] != null && s["task"].ToString().Contains(rule.TaskName))
                    && s.SelectToken("enabled", false)?.ToString() != "false"));

            if (!result && steps
                    .Any(s => s["template"] != null))
                return null;

            return result;
        }
    }
}