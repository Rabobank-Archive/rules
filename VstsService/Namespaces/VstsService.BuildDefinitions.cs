using System;
using System.Collections.Generic;
using System.Text;

namespace VstsService
{
    namespace BuildDefinition
    {
        public class Definition
        {
            public string id { get; set; }
        }

        public class Inputs
        {
            public string branchFilters { get; set; }
            public string additionalFields { get; set; }
            public string workItemType { get; set; }
            public string assignToRequestor { get; set; }
        }

        public class Option
        {
            public bool enabled { get; set; }
            public Definition definition { get; set; }
            public Inputs inputs { get; set; }
        }

        public class Trigger
        {
            public List<string> branchFilters { get; set; }
            public List<object> pathFilters { get; set; }
            public bool batchChanges { get; set; }
            public int maxConcurrentBuildsPerBranch { get; set; }
            public int pollingInterval { get; set; }
            public string triggerType { get; set; }
        }

        public class BuildConfiguration
        {
            public string value { get; set; }
            public bool allowOverride { get; set; }
        }

        public class BuildPlatform
        {
            public string value { get; set; }
            public bool allowOverride { get; set; }
        }

        public class SystemDebug
        {
            public string value { get; set; }
            public bool allowOverride { get; set; }
        }

        public class Variables
        {
            public BuildConfiguration BuildConfiguration { get; set; }
            public BuildPlatform BuildPlatform { get; set; }
            //public SystemDebug __invalid_name__system.debug { get; set; }
        }

        public class RetentionRule
        {
            public List<string> branches { get; set; }
            public List<object> artifacts { get; set; }
            public List<string> artifactTypesToDelete { get; set; }
            public int daysToKeep { get; set; }
            public int minimumToKeep { get; set; }
            public bool deleteBuildRecord { get; set; }
            public bool deleteTestResults { get; set; }
        }

        public class Properties
        {
        }

        public class Self
        {
            public string href { get; set; }
        }

        public class Web
        {
            public string href { get; set; }
        }

        public class Editor
        {
            public string href { get; set; }
        }

        public class Badge
        {
            public string href { get; set; }
        }

        public class Links
        {
            public Self self { get; set; }
            public Web web { get; set; }
            public Editor editor { get; set; }
            public Badge badge { get; set; }
        }

        public class Environment
        {
        }

        public class Task
        {
            public string id { get; set; }
            public string versionSpec { get; set; }
            public string definitionType { get; set; }
        }

        public class Inputs2
        {
            public string command { get; set; }
            public string publishWebProjects { get; set; }
            public string projects { get; set; }
            public string custom { get; set; }
            public string arguments { get; set; }
            public string publishTestResults { get; set; }
            public string zipAfterPublish { get; set; }
            public string modifyOutputPath { get; set; }
            public string selectOrConfig { get; set; }
            public string feedRestore { get; set; }
            public string includeNuGetOrg { get; set; }
            public string nugetConfigPath { get; set; }
            public string externalEndpoints { get; set; }
            public string noCache { get; set; }
            public string packagesDirectory { get; set; }
            public string verbosityRestore { get; set; }
            public string searchPatternPush { get; set; }
            public string nuGetFeedType { get; set; }
            public string feedPublish { get; set; }
            public string externalEndpoint { get; set; }
            public string searchPatternPack { get; set; }
            public string configurationToPack { get; set; }
            public string outputDir { get; set; }
            public string nobuild { get; set; }
            public string versioningScheme { get; set; }
            public string versionEnvVar { get; set; }
            public string requestedMajorVersion { get; set; }
            public string requestedMinorVersion { get; set; }
            public string requestedPatchVersion { get; set; }
            public string buildProperties { get; set; }
            public string verbosityPack { get; set; }
            public string workingDirectory { get; set; }
            public string PathtoPublish { get; set; }
            public string ArtifactName { get; set; }
            public string ArtifactType { get; set; }
            public string TargetPath { get; set; }
            public string Parallel { get; set; }
            public string ParallelCount { get; set; }
        }

        public class Step
        {
            public Environment environment { get; set; }
            public bool enabled { get; set; }
            public bool continueOnError { get; set; }
            public bool alwaysRun { get; set; }
            public string displayName { get; set; }
            public int timeoutInMinutes { get; set; }
            public Task task { get; set; }
            public Inputs2 inputs { get; set; }
        }

        public class ExecutionOptions
        {
            public int type { get; set; }
        }

        public class Target
        {
            public ExecutionOptions executionOptions { get; set; }
            public bool allowScriptsAuthAccessOption { get; set; }
            public int type { get; set; }
        }

        public class Phase
        {
            public List<Step> steps { get; set; }
            public string name { get; set; }
            public string refName { get; set; }
            public string condition { get; set; }
            public Target target { get; set; }
            public string jobAuthorizationScope { get; set; }
            public int jobCancelTimeoutInMinutes { get; set; }
        }

        public class Process
        {
            public List<Phase> phases { get; set; }
            public int type { get; set; }
        }

        public class Properties2
        {
            public string cleanOptions { get; set; }
            public string labelSources { get; set; }
            public string labelSourcesFormat { get; set; }
            public string reportBuildStatus { get; set; }
            public string gitLfsSupport { get; set; }
            public string skipSyncSource { get; set; }
            public string checkoutNestedSubmodules { get; set; }
            public string fetchDepth { get; set; }
        }

        public class Repository
        {
            public Properties2 properties { get; set; }
            public string id { get; set; }
            public string type { get; set; }
            public string name { get; set; }
            public string url { get; set; }
            public string defaultBranch { get; set; }
            public string clean { get; set; }
            public bool checkoutSubmodules { get; set; }
        }

        public class Options
        {
        }

        public class Properties3
        {
        }

        public class Input
        {
            public List<object> aliases { get; set; }
            public Options options { get; set; }
            public Properties3 properties { get; set; }
            public string name { get; set; }
            public string label { get; set; }
            public string defaultValue { get; set; }
            public string type { get; set; }
            public string helpMarkDown { get; set; }
            public string visibleRule { get; set; }
            public string groupName { get; set; }
        }

        public class ProcessParameters
        {
            public List<Input> inputs { get; set; }
        }

        public class Avatar
        {
            public string href { get; set; }
        }

        public class Links2
        {
            public Avatar avatar { get; set; }
        }

        public class AuthoredBy
        {
            public string displayName { get; set; }
            public string url { get; set; }
            public Links2 _links { get; set; }
            public string id { get; set; }
            public string uniqueName { get; set; }
            public string imageUrl { get; set; }
            public string descriptor { get; set; }
        }

        public class Self2
        {
            public string href { get; set; }
        }

        public class Links3
        {
            public Self2 self { get; set; }
        }

        public class Pool
        {
            public int id { get; set; }
            public string name { get; set; }
            public bool isHosted { get; set; }
        }

        public class Queue
        {
            public Links3 _links { get; set; }
            public int id { get; set; }
            public string name { get; set; }
            public string url { get; set; }
            public Pool pool { get; set; }
        }

        public class Project
        {
            public string id { get; set; }
            public string name { get; set; }
            public string description { get; set; }
            public string url { get; set; }
            public string state { get; set; }
            public int revision { get; set; }
            public string visibility { get; set; }
        }

        public class RootObject
        {
            public List<Option> options { get; set; }
            public List<Trigger> triggers { get; set; }
            public Variables variables { get; set; }
            public List<RetentionRule> retentionRules { get; set; }
            public Properties properties { get; set; }
            public List<object> tags { get; set; }
            public Links _links { get; set; }
            public string buildNumberFormat { get; set; }
            public string jobAuthorizationScope { get; set; }
            public int jobTimeoutInMinutes { get; set; }
            public int jobCancelTimeoutInMinutes { get; set; }
            public Process process { get; set; }
            public Repository repository { get; set; }
            public ProcessParameters processParameters { get; set; }
            public string quality { get; set; }
            public AuthoredBy authoredBy { get; set; }
            public List<object> drafts { get; set; }
            public Queue queue { get; set; }
            public int id { get; set; }
            public string name { get; set; }
            public string url { get; set; }
            public string uri { get; set; }
            public string path { get; set; }
            public string type { get; set; }
            public string queueStatus { get; set; }
            public int revision { get; set; }
            public DateTime createdDate { get; set; }
            public Project project { get; set; }
        }
    }
}