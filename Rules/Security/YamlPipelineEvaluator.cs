using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Response;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using Repository = SecurePipelineScan.VstsService.Requests.Repository;

namespace SecurePipelineScan.Rules.Security
{
    public class YamlPipelineEvaluator : IPipelineEvaluator
    {
        private readonly IVstsRestClient _client;
        private const int MaxLevel = 5;
        private const string RepoGitType = "git";
        private const string TemplatePropertyName = "template";

        public YamlPipelineEvaluator(IVstsRestClient client)
        {
            _client = client;
        }

        public async Task<bool?> EvaluateAsync(Project project, BuildDefinition buildPipeline,
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

            if (!buildPipeline.Repository.Url.ToString().ToUpperInvariant()
                .Contains(project.Name.ToUpperInvariant()))
            {
                return false;
            }

            if (rule == null)
            {
                throw new ArgumentNullException(nameof(rule));
            }

            var yamlPipeline = await GetGitYamlItemAsync(new ResourceRef(project.Id, buildPipeline.Repository.Id)
                .WithFilePath(buildPipeline.Process.YamlFilename)
            ).ConfigureAwait(false);
            return yamlPipeline.Any() ? PipelineContainsValidTask(yamlPipeline, rule) : false;
        }

        private async Task<JToken> GetGitYamlItemAsync(ResourceRef resourceRef, int nestingLevel = 0)
        {
            // to avoid hanging on circular references or spending too much time on resolving
            // deeply nested files we limit the max nesting level (to 3 in this case).
            if (nestingLevel > MaxLevel)
            {
                throw new InvalidOperationException("template nesting level overflow");
            }

            nestingLevel++;

            var request = Repository.GitItem(resourceRef.ProjectId, resourceRef.RepoId, resourceRef.GetFilePath())
                .AsJson();
            var gitItem = await _client.GetAsync(request).ConfigureAwait(false);

            var yamlContent = gitItem?.SelectToken("content", false)?.ToString();
            if (yamlContent == null)
            {
                return new JObject();
            }

            var yamlPipeline = ConvertYamlToJson(yamlContent);
            if (!yamlPipeline.Any())
            {
                return yamlPipeline;
            }

            yamlPipeline = await ResolveTemplatesAsync(GetJobs, yamlPipeline, resourceRef, nestingLevel)
                .ConfigureAwait(false);
            yamlPipeline = await ResolveTemplatesAsync(GetSteps, yamlPipeline, resourceRef, nestingLevel)
                .ConfigureAwait(false);
            return yamlPipeline;
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

        private async Task<JToken> ResolveTemplatesAsync(Func<JToken, IEnumerable<JToken>> getItems,
            JToken yamlPipeline, ResourceRef parentResourceRef, int level)
        {
            var toRemove = new List<JToken>();
            var toAdd = new List<(JToken itemToAdd, JContainer parentContainer)>();

            foreach (var item in getItems(yamlPipeline))
            {
                if (item[TemplatePropertyName] == null ||
                    string.IsNullOrWhiteSpace(item[TemplatePropertyName].Value<string>()))
                {
                    continue;
                }

                var intResourceRef = new ResourceRef(parentResourceRef.ProjectId, parentResourceRef.RepoId)
                    .WithFilePath(item[TemplatePropertyName].Value<string>());

                var extResourceRef = GetNextResourceRef(intResourceRef, yamlPipeline);
                var actualResourceRef = SelectResourceRef(intResourceRef, extResourceRef);
                actualResourceRef.Parent = parentResourceRef;
                var yamlTemplate = await GetGitYamlItemAsync(actualResourceRef, level).ConfigureAwait(false);

                if (!yamlTemplate.Any())
                {
                    continue;
                }

                var nestedItems = getItems(yamlTemplate);
                if (nestedItems == null)
                {
                    continue;
                }

                toAdd.AddRange(nestedItems.Select(nestedItem => (
                    itemToAdd: nestedItem,
                    // the nested steps in the template must be added to the parent of the template step
                    parentContainer: item.Parent
                )));

                toRemove.Add(item);
            }

            foreach (var (itemToAdd, parentContainer) in toAdd)
            {
                parentContainer.Add(itemToAdd.DeepClone());
            }

            foreach (var itemToRemove in toRemove)
            {
                itemToRemove.Remove();
            }

            return yamlPipeline;
        }

        private static ResourceRef SelectResourceRef(ResourceRef internalRef, ResourceRef externalRef)
        {
            return externalRef != null && externalRef.RepoType == RepoGitType
                ? externalRef
                : internalRef;
        }

        private static ResourceRef GetNextResourceRef(ResourceRef resourceRef, JToken yamlPipeline)
        {
            var yamlFileName = resourceRef.GetFilePath();
            if (!yamlFileName.Contains('@'))
            {
                return default;
            }

            var segments = yamlFileName.Split('@');
            const int maxSegmentCount = 2;
            if (segments.Length != maxSegmentCount)
            {
                throw new InvalidOperationException("yaml name with repo reference should have exactly two segments");
            }

            var yamlName = segments[0];
            var repoRefAlias = segments[1];

            var repoRefs = GetRepoReferences(resourceRef, yamlPipeline);
            if (!repoRefs.ContainsKey(repoRefAlias))
            {
                throw new InvalidOperationException("repo alias not found");
            }

            return new ResourceRef(
                repoRefs[repoRefAlias].ProjectId,
                repoRefs[repoRefAlias].RepoId,
                repoRefs[repoRefAlias].RepoType).WithFilePath(yamlName);
        }

        private static Dictionary<string, ResourceRef> GetRepoReferences(ResourceRef resourceRef, JToken yamlPipeline)
        {
            var result = new Dictionary<string, ResourceRef>();
            var repos = yamlPipeline["resources"]?["repositories"];
            if (repos == null)
            {
                return result;
            }

            foreach (var repo in repos)
            {
                var segments = repo["name"].Value<string>().Split('/');
                string projectId;
                string repoId;

                const int twoMeansWeHaveProjectAndRepo = 2;
                const int oneMeansWeHaveRepoOnly = 1;
                if (segments.Length == twoMeansWeHaveProjectAndRepo)
                {
                    projectId = segments[0];
                    repoId = segments[1];
                }
                else if (segments.Length == oneMeansWeHaveRepoOnly)
                {
                    projectId = resourceRef.ProjectId;
                    repoId = segments[0];
                }
                else
                {
                    throw new InvalidOperationException("repo name contains an unsupported number of segments");
                }

                result[repo["repository"].Value<string>()] =
                    new ResourceRef(projectId,
                        repoId,
                        repo["type"].Value<string>());
            }

            return result;
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

        private static bool VerifyRuleInputs(Dictionary<string, object> inputs, JToken pipelineInputs)
        {
            foreach (var (ruleInputKey, ruleInputValue) in inputs)
            {
                var pipelineInput = pipelineInputs[ruleInputKey];
                if (pipelineInput == null)
                {
                    return false;
                }

                var value = pipelineInput.ToString();
                if (!TryParse(value, out var result))
                {
                    return false;
                }

                if (!result.Equals(ruleInputValue))
                {
                    return false;
                }
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

        private static bool TryParse(string value, out object result)
        {
            if (bool.TryParse(value, out var result2))
            {
                result = result2;
                return true;
            }

            result = null;
            return false;
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

        private static IEnumerable<JToken> GetJobs(JToken yamlPipeline)
        {
            var jobs = new List<JToken>();
            jobs.AddRange(yamlPipeline.SelectTokens("jobs[*]"));
            jobs.AddRange(yamlPipeline.SelectTokens("stages[*].jobs[*]"));
            return jobs;
        }

        private class ResourceRef
        {
            private string[] _path;
            private string _fileName;

            public ResourceRef(string projectId, string repoId, string repoType = null)
            {
                ProjectId = projectId;
                RepoId = repoId;
                RepoType = repoType;
            }

            public string RepoType { get; }
            public string ProjectId { get; }
            public string RepoId { get; }
            public ResourceRef Parent { get; set; }

            private string[] GetPath()
            {
                // if we are the root if we reference a yaml file from another repo then the current path context
                // should be forgotten and we start from the root again.
                return (Parent == null || Parent.ProjectId != ProjectId || Parent.RepoId != RepoId
                    ? new string[0]
                    : Parent.GetPath()).Concat(_path).ToArray();
            }

            public ResourceRef WithFilePath(string filePath)
            {
                var segments = filePath.Split('/');
                if (segments.Length > 1)
                {
                    _path = segments.Take(segments.Length - 1).ToArray();
                    _fileName = segments.Last();
                }
                else if (segments.Length == 1)
                {
                    _path = new string[0];
                    _fileName = segments.Last();
                }
                else
                {
                    throw new InvalidOperationException("invalid path");
                }

                return this;
            }

            public string GetFilePath()
            {
                var path = string.Join("/", GetPath());
                var filePath = $"{path}/{_fileName}";
                return filePath.Trim('/');
            }
        }
    }
}