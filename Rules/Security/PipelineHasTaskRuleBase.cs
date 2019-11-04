using SecurePipelineScan.VstsService.Response;
using System.Linq;
using System.Threading.Tasks;
using System;
using SecurePipelineScan.VstsService;
using Newtonsoft.Json.Linq;
using System.IO;
using YamlDotNet.Serialization;
using Newtonsoft.Json;

namespace SecurePipelineScan.Rules.Security
{
    public abstract class PipelineHasTaskRuleBase
    {
        readonly IVstsRestClient _client;

        protected PipelineHasTaskRuleBase(IVstsRestClient client)
        {
            _client = client;
        }

        protected abstract string TaskId { get; }
        protected abstract string TaskName { get; }
        protected abstract string StepName { get; }

        private const int GuiPipelineProcessType = 1;
        private const int YamlPipelineProcessType = 2;
        private const string MavenTaskId = "ac4ee482-65da-4485-a532-7b085873e532";

        public async Task<bool?> EvaluateAsync(Project project, BuildDefinition buildPipeline)
        {
            if (project == null)
                throw new ArgumentNullException(nameof(project));
            if (buildPipeline == null)
                throw new ArgumentNullException(nameof(buildPipeline));

            if (buildPipeline.Process.Type == GuiPipelineProcessType)
                return EvaluateGuiPipeline(buildPipeline);
            else if (buildPipeline.Process.Type == YamlPipelineProcessType)
                return await EvaluateYamlPipelineAsync(project, buildPipeline)
                    .ConfigureAwait(false);
            else
                return null;
        }

        private bool? EvaluateGuiPipeline(BuildDefinition buildPipeline)
        {
            if (buildPipeline.Process.Phases == null)
                throw new ArgumentOutOfRangeException(nameof(buildPipeline));

            bool? result = DoesGuiPipelineContainTask(buildPipeline, TaskId);

            if (!result.GetValueOrDefault() && DoesGuiPipelineContainTask(buildPipeline, MavenTaskId))
                result = null;

            return result;
        }

        private static bool DoesGuiPipelineContainTask(BuildDefinition buildPipeline, string taskId) =>
            buildPipeline.Process.Phases
                .Where(p => p.Steps != null)
                .SelectMany(p => p.Steps)
                .Any(s => s.Enabled && s.Task.Id == taskId);

        private async Task<bool?> EvaluateYamlPipelineAsync(Project project,
            BuildDefinition buildPipeline)
        {
            if (buildPipeline.Process.YamlFilename == null)
                throw new ArgumentOutOfRangeException(nameof(buildPipeline));

            if (!buildPipeline.Repository.Url.ToString().ToUpperInvariant()
                    .Contains(project.Name.ToUpperInvariant()))
                return false;

            var yamlPipeline = await GetYamlPipelineAsync(project.Id, buildPipeline.Repository.Id,
                    buildPipeline.Process.YamlFilename)
                .ConfigureAwait(false);

            if (yamlPipeline == null)
                return false;

            return DoesYamlPipelineContainTask(yamlPipeline);
        }

        private async Task<JObject> GetYamlPipelineAsync(string projectId, string repositoryId,
            string yamlFileName)
        {
            var gitItem = await _client.GetAsync(VstsService.Requests.Repository.GitItem(
                projectId, repositoryId, yamlFileName)
                .AsJson()).ConfigureAwait(false);

            if (gitItem == null)
                return null;

            var yamlContent = gitItem.SelectToken("content", false)?.ToString();
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
            catch (JsonReaderException)
            {
                // parse exceptions are handled as unknown rule result
                return null;
            }
        }

        private bool? DoesYamlPipelineContainTask(JToken yamlPipeline)
        {
            var result = yamlPipeline.SelectTokens("steps[*]")
                .Any(s => (s[StepName] != null ||
                    (s["task"] != null && s["task"].ToString().Contains(TaskName))
                    && s.SelectToken("enabled", false)?.ToString() != "false"));

            if (!result && yamlPipeline.SelectTokens("steps[*]")
                    .Any(s => s["template"] != null))
                return null;

            return result;
        }
    }
}