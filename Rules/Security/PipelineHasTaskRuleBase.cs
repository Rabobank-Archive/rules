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

        protected abstract string TaskId { get; }
        protected abstract string TaskName { get; }
        private const string MavenTaskId = "ac4ee482-65da-4485-a532-7b085873e532";

        public async Task<bool?> EvaluateAsync(BuildDefinition buildPipeline)
        {
            if (buildPipeline == null)
                throw new ArgumentNullException(nameof(buildPipeline));

            bool? result = null;
            if (buildPipeline.Process.Type == 1)
            {
                // process.type 1 is a gui pipeline, 2 is a yaml pipeline
                if (buildPipeline.Process.Phases == null)
                    throw new ArgumentOutOfRangeException(nameof(buildPipeline));

                result = DoesPipelineContainTask(buildPipeline, TaskId);

                if (!result.GetValueOrDefault() && DoesPipelineContainTask(buildPipeline, MavenTaskId))
                    result = null;
            }
            else if (buildPipeline.Process.Type == 2)
            { 
                if(buildPipeline.Process.YamlFilename == null)
                    throw new ArgumentOutOfRangeException(nameof(buildPipeline));

                result = await DoesPipelineContainTaskYml(buildPipeline, TaskName);
            }

            return await Task.FromResult(result);
        }

        private static bool DoesPipelineContainTask(BuildDefinition buildPipeline, string taskId) => 
            buildPipeline.Process.Phases
                .Where(p => p.Steps != null)
                .SelectMany(p => p.Steps)
                .Any(s => s.Enabled && s.Task.Id == taskId);

        private async Task<bool?> DoesPipelineContainTaskYml(BuildDefinition buildPipeline, string TaskName)
        {
            
            var gitItem = await _client.GetAsync(VstsService.Requests.Repository.GitItem(buildPipeline.Project.Id,
                buildPipeline.Repository.Id, buildPipeline.Process.YamlFilename)
                .AsJson());

            if(gitItem == null)
                throw new ArgumentNullException(nameof(gitItem));

            var jsonObject = YamlToJson(gitItem);

            return jsonObject.SelectTokens("steps[*]")
                .Any(s => s.SelectToken("task", false)?.ToString() == TaskName
                    && s.SelectToken("enabled", false)?.ToString() != "false") || s.SelectToken("publish", false)?.ToString() != null;
        }

        private JObject YamlToJson(JObject gitItem)
        {
            var yamlText = gitItem.SelectToken("content").ToString();
            var yamlObject = new Deserializer().Deserialize(new StringReader(yamlText));

            var jsonText = JsonConvert.SerializeObject(yamlObject);
            return JsonConvert.DeserializeObject<JObject>(jsonText);
        }
    }
}