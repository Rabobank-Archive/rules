using SecurePipelineScan.VstsService.Response;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
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
        private const int MaxLevel = 3;

        public YamlPipelineEvaluator(IVstsRestClient client)
        {
            _client = client;
        }

        public async Task<bool?> EvaluateAsync(Project project, BuildDefinition buildPipeline,
            IPipelineHasTaskRule rule)
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

            var yamlPipeline = await GetGitYamlItemAsync(project.Id, buildPipeline.Repository.Id,
                    buildPipeline.Process.YamlFilename, 0)
                .ConfigureAwait(false);

            if (yamlPipeline == null)
                return false;

            return PipelineContainsTask(yamlPipeline, rule);
        }

        private async Task<JToken> GetGitYamlItemAsync(string projectId, string repositoryId,
            string yamlFileName, int nestingLevel)
        {
            // to avoid hanging on circular references or spending too much time on resolving
            // deeply nested files we limit the max nesting level (to 3 inthis case).
            if (nestingLevel > MaxLevel)
                return null;

            nestingLevel++;

            var request = VstsService.Requests.Repository.GitItem(projectId, repositoryId, yamlFileName).AsJson();
            var gitItem = await _client.GetAsync(request).ConfigureAwait(false);

            var yamlContent = gitItem?.SelectToken("content", false)?.ToString();
            if (yamlContent == null)
                return null;

            return await ConvertYamlToJsonAsync(yamlContent, projectId, repositoryId, nestingLevel).ConfigureAwait(false);
        }

        private async Task<JToken> ConvertYamlToJsonAsync(string yamlText, string projectId, string repositoryId, int level)
        {
            try
            {
                var deserializer = new DeserializerBuilder().Build();
                var yamlObject = deserializer.Deserialize(new StringReader(yamlText));

                var serializer = new SerializerBuilder()
                    .JsonCompatible()
                    .Build();

                var json = serializer.Serialize(yamlObject);
                var yamlPipeline = JsonConvert.DeserializeObject<JObject>(json);
                return await ResolveTemplatesAsync(yamlPipeline, projectId, repositoryId, level).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex is SyntaxErrorException || ex is InvalidCastException || ex is YamlException)
            {
                return null;
            }
        }

        private async Task<JToken> ResolveTemplatesAsync(JToken yamlPipeline, string projectId, string repositoryId, int level)
        {
            // cannot mutate a collection while we are iterating it
            var stepsToRemove = new List<JToken>();
            var stepsToAdd = new List<(JToken stepToAdd, JContainer parentContainer)>();

            var steps = GetSteps(yamlPipeline);
            foreach (var step in steps)
            {
                // no step templates to resolve, let's continue
                if (step["template"] == null)
                    continue;

                var yamlFileName = step["template"].Value<string>();
                if (string.IsNullOrWhiteSpace(yamlFileName))
                    continue;

                var yamlTemplate = await GetGitYamlItemAsync(projectId, repositoryId, yamlFileName, level)
                    .ConfigureAwait(false);
                if (yamlTemplate == null)
                    continue;

                var nestedSteps = GetSteps(yamlTemplate);
                if (nestedSteps == null)
                    continue;

                stepsToAdd.AddRange(nestedSteps.Select(nestedStep => (
                    stepToAdd: nestedStep,
                    // the nested steps in the template must be added to the parent of the template step
                    parentContainer: step.Parent
                )));

                stepsToRemove.Add(step);
            }

            foreach (var (stepToAdd, parentContainer) in stepsToAdd)
            {
                parentContainer.Add(stepToAdd.DeepClone());
            }

            foreach (var stepTuple in stepsToRemove)
            {
                stepTuple.Remove();
            }

            return yamlPipeline;
        }

        private static bool? PipelineContainsTask(JToken yamlPipeline, IPipelineHasTaskRule rule)
        {
            var steps = GetSteps(yamlPipeline).ToList();
            var result = steps
                .Any(s => (s[rule.StepName] != null ||
                           (s["task"] != null && ContainsTaskName(s["task"].ToString(), rule.TaskName))
                           && s.SelectToken("enabled", false)?.ToString() != "false"));

            if (!result && steps
                    .Any(s => s["template"] != null))
                return null;

            return result;
        }

        private static bool ContainsTaskName(string task, string name)
        {
            var segments = task.Split('@');
            return segments[0] == name;
        }

        private static IEnumerable<JToken> GetSteps(JToken yamlPipeline)
        {
            var steps = yamlPipeline.SelectTokens("steps[*]");
            if (yamlPipeline["jobs"] != null)
                steps = yamlPipeline.SelectTokens("jobs[*].steps[*]");
            return steps;
        }
    }
}