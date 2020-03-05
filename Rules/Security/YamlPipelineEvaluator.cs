using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Flurl.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Requests;
using SecurePipelineScan.VstsService.Response;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace SecurePipelineScan.Rules.Security
{
    public class YamlPipelineEvaluator : IPipelineEvaluator
    {
        private readonly IVstsRestClient _client;

        public YamlPipelineEvaluator(IVstsRestClient client)
        {
            _client = client;
        }

        public async Task<bool?> EvaluateAsync(VstsService.Response.Project project, BuildDefinition buildPipeline,
            IPipelineHasTaskRule rule)
        {
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }

            if (buildPipeline == null)
            {
                throw new ArgumentNullException(nameof(buildPipeline));
            }

            if (buildPipeline.Process.YamlFilename == null)
            {
                throw new ArgumentOutOfRangeException(nameof(buildPipeline));
            }

            if (rule == null)
            {
                throw new ArgumentNullException(nameof(rule));
            }

            try
            {
                var response = await _client.PostAsync(YamlPipeline.Parse(project.Id, buildPipeline.Id),
                    new YamlPipeline.YamlPipelineRequest()
                ).ConfigureAwait(false);

                if (response?.FinalYaml == null)
                    return false;

                var yamlPipeline = ConvertYamlToJson(response?.FinalYaml);

                return yamlPipeline.Any() && PipelineContainsValidTask(yamlPipeline, rule);
            }
            catch (FlurlHttpException e)
            {
                return e.Call.HttpStatus == HttpStatusCode.BadRequest ? false : (bool?)null;
            }
        }

        private static JToken ConvertYamlToJson(string yamlText)
        {
            try
            {
                var deserializer = new DeserializerBuilder().Build();
                var yamlObject = deserializer.Deserialize(new StringReader(yamlText));

                var serializer = new SerializerBuilder()
                    .JsonCompatible()
                    .Build();

                var json = serializer.Serialize(yamlObject);
                return JsonConvert.DeserializeObject<JToken>(json);
            }
            catch (Exception ex) when (ex is SyntaxErrorException || ex is InvalidCastException || ex is YamlException)
            {
                return new JObject();
            }
        }

        private static bool PipelineContainsValidTask(JToken yamlPipeline, IPipelineHasTaskRule rule)
        {
            foreach (var step in GetSteps(yamlPipeline))
            {
                if (!string.IsNullOrWhiteSpace(rule.StepName) && step[rule.StepName] != null)
                {
                    return true;
                }

                if (!GetTaskFromStep(step, rule.TaskName))
                {
                    continue;
                }

                if (rule.Inputs == null || !rule.Inputs.Any())
                {
                    return true;
                }

                var pipelineInputs = step["inputs"];
                return pipelineInputs != null && VerifyRuleInputs(rule.Inputs, pipelineInputs);
            }

            return false;
        }

        internal static bool ContainsTaskName(string fullTaskName, string name)
        {
            var taskNameWithPrefix = fullTaskName.Split('@')[0];
            var taskName = taskNameWithPrefix.Split('.').Last();
            return taskName == name;
        }

        private static bool VerifyRuleInputs(Dictionary<string, string> inputs, JToken pipelineInputs)
        {
            foreach (var (ruleInputKey, ruleInputValue) in inputs)
            {
                var pipelineInput = pipelineInputs[ruleInputKey];
                if (pipelineInput == null)
                    return false;

                if (pipelineInput?.ToString() != ruleInputValue)
                    return false;
            }

            return true;
        }

        private static bool GetTaskFromStep(JToken step, string taskName)
        {
            var task = step["task"];
            return task != null &&
                   ContainsTaskName(task.ToString(), taskName) &&
                   step.SelectToken("enabled", false)?.ToString() != "false";
        }

        private static IEnumerable<JToken> GetSteps(JToken yamlPipeline)
        {
            var steps = new List<JToken>();
            steps.AddRange(yamlPipeline.SelectTokens("steps[*]"));
            steps.AddRange(yamlPipeline.SelectTokens("jobs[*].steps[*]"));
            steps.AddRange(yamlPipeline.SelectTokens("stages[*].steps[*]"));
            steps.AddRange(yamlPipeline.SelectTokens("stages[*].jobs[*].steps[*]"));
            return steps;
        }
    }
}