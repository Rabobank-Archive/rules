using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace SecurePipelineScan.Rules
{
    internal static class ReleaseDefinitionHelper
    {
        public static IEnumerable<string> GetArtifactAliases(JToken definition)
        {
            var artifacts = definition.SelectTokens("artifacts[?(@.type == 'Build')]");
            return artifacts.Select(a => a["alias"].ToString());
        }

        public static void AddConditionToEnvironments(JToken definition, string artifact, string stageId)
        {
            var environments = definition.SelectTokens($"environments[?(@.id == {stageId})]");
            foreach (var environment in environments)
            {
                var conditions = environment["conditions"].ToList();

                if (conditions.Any(c =>
                    (string) c["name"] == artifact && c["value"].Value<string>().Contains("master", StringComparison.InvariantCulture)))
                {
                    continue;
                }

                conditions.Add(JToken.FromObject(new
                {
                    name = artifact,
                    conditionType = "artifact",
                    value =
                        "{\"sourceBranch\":\"master\",\"tags\":[],\"useBuildDefinitionBranch\":false,\"createReleaseOnBuildTagging\":false}"
                }));

                environment["conditions"] = JArray.FromObject(conditions);
            }
        }
    }
}