using SecurePipelineScan.VstsService.Response;
using System.Linq;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;
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

        private readonly int GuiPipelineProcessType = 1;
        private readonly int YamlPipelineProcessType = 2;

        protected abstract string TaskId { get; }
        protected abstract string TaskName { get; }
        protected abstract string StepName { get; }

        private const string MavenTaskId = "ac4ee482-65da-4485-a532-7b085873e532";

        public async Task<bool?> EvaluateAsync(Project project, BuildDefinition buildPipeline)
        {
            if (project == null)
                throw new ArgumentNullException(nameof(project));
            if (buildPipeline == null)
                throw new ArgumentNullException(nameof(buildPipeline));

            bool? result;
            if (buildPipeline.Process.Type == GuiPipelineProcessType)
            {
                if (buildPipeline.Process.Phases == null)
                    throw new ArgumentOutOfRangeException(nameof(buildPipeline));

                result = DoesGuiPipelineContainTask(buildPipeline, TaskId);

                if (!result.GetValueOrDefault() && DoesGuiPipelineContainTask(buildPipeline, MavenTaskId))
                    result = null;
            }
            else if (buildPipeline.Process.Type == YamlPipelineProcessType)
            {
                if (buildPipeline.Process.YamlFilename == null)
                    throw new ArgumentOutOfRangeException(nameof(buildPipeline));

                result = await DoesYamlPipelineContainTaskAsync(project, buildPipeline)
                    .ConfigureAwait(false);
            }
            else
                result = null;

            return result;
        }

        private static bool DoesGuiPipelineContainTask(BuildDefinition buildPipeline, string taskId) => 
            buildPipeline.Process.Phases
                .Where(p => p.Steps != null)
                .SelectMany(p => p.Steps)
                .Any(s => s.Enabled && s.Task.Id == taskId);

        private async Task<bool?> DoesYamlPipelineContainTaskAsync(Project project,
            BuildDefinition buildPipeline) 
        {
            if (!buildPipeline.Repository.Url.ToString().ToUpperInvariant()
                    .Contains(project.Name.ToUpperInvariant()))
                return false;

            var yamlPipeline = await GetYamlPipelineAsync(project.Id, buildPipeline.Repository.Id,
                    buildPipeline.Process.YamlFilename)
                .ConfigureAwait(false);

            return EvaluateYamlPipeline(yamlPipeline);
        }
               
        private async Task<JObject> GetYamlPipelineAsync(string projectId, string repositoryId, 
            string yamlFileName)
        {
            var gitItem = await _client.GetAsync(VstsService.Requests.Repository.GitItem(
                projectId, repositoryId, yamlFileName)
                .AsJson()).ConfigureAwait(false);

            if (gitItem == null)
                throw new ArgumentNullException(nameof(gitItem));

            var yamlContent = gitItem.SelectToken("content", false)?.ToString();
            return ConvertYamlToJson(yamlContent);
        }

        private static JObject ConvertYamlToJson(string yamlText)
        {
            var yamlObject = new Deserializer().Deserialize(new StringReader(yamlText));
            var jsonText = JsonConvert.SerializeObject(yamlObject);
            return JsonConvert.DeserializeObject<JObject>(jsonText);
        }

        private bool? EvaluateYamlPipeline(JToken yamlPipeline)
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