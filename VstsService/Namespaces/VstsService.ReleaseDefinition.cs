using System;
using System.Collections.Generic;
using System.Text;

namespace VstsService
{
    namespace ReleaseDefinition
    {

        public class Avatar
        {
            public string href { get; set; }
        }

        public class Links
        {
            public Avatar avatar { get; set; }
        }

        public class CreatedBy
        {
            public string displayName { get; set; }
            public string url { get; set; }
            public Links _links { get; set; }
            public string id { get; set; }
            public string uniqueName { get; set; }
            public string imageUrl { get; set; }
            public string descriptor { get; set; }
        }

        public class Avatar2
        {
            public string href { get; set; }
        }

        public class Links2
        {
            public Avatar2 avatar { get; set; }
        }

        public class ModifiedBy
        {
            public string displayName { get; set; }
            public string url { get; set; }
            public Links2 _links { get; set; }
            public string id { get; set; }
            public string uniqueName { get; set; }
            public string imageUrl { get; set; }
            public string descriptor { get; set; }
        }

        public class AppServiceName
        {
            public string value { get; set; }
        }

        public class Variables
        {
            public AppServiceName AppServiceName { get; set; }
        }

        public class Avatar3
        {
            public string href { get; set; }
        }

        public class Links3
        {
            public Avatar3 avatar { get; set; }
        }

        public class Owner
        {
            public string displayName { get; set; }
            public string url { get; set; }
            public Links3 _links { get; set; }
            public string id { get; set; }
            public string uniqueName { get; set; }
            public string imageUrl { get; set; }
            public string descriptor { get; set; }
        }

        public class AppServicePlan
        {
            public string value { get; set; }
        }

        public class ResourceGroupName
        {
            public string value { get; set; }
        }

        public class AppServiceName2
        {
            public string value { get; set; }
        }

        public class Variables2
        {
            public AppServicePlan AppServicePlan { get; set; }
            public ResourceGroupName ResourceGroupName { get; set; }
            public AppServiceName2 AppServiceName { get; set; }
        }

        public class Avatar4
        {
            public string href { get; set; }
        }

        public class Links4
        {
            public Avatar4 avatar { get; set; }
        }

        public class Approver
        {
            public string displayName { get; set; }
            public string url { get; set; }
            public Links4 _links { get; set; }
            public string id { get; set; }
            public string uniqueName { get; set; }
            public string imageUrl { get; set; }
            public bool isContainer { get; set; }
            public string descriptor { get; set; }
        }

        public class Approval
        {
            public int rank { get; set; }
            public bool isAutomated { get; set; }
            public bool isNotificationOn { get; set; }
            public int id { get; set; }
            public Approver approver { get; set; }
        }

        public class ApprovalOptions
        {
            public object requiredApproverCount { get; set; }
            public bool releaseCreatorCanBeApprover { get; set; }
            public bool autoTriggeredAndPreviousEnvironmentApprovedCanBeSkipped { get; set; }
            public bool enforceIdentityRevalidation { get; set; }
            public int timeoutInMinutes { get; set; }
            public string executionOrder { get; set; }
        }

        public class PreDeployApprovals
        {
            public List<Approval> approvals { get; set; }
            public ApprovalOptions approvalOptions { get; set; }
        }

        public class DeployStep
        {
            public int id { get; set; }
        }

        public class Approval2
        {
            public int rank { get; set; }
            public bool isAutomated { get; set; }
            public bool isNotificationOn { get; set; }
            public int id { get; set; }
        }

        public class ApprovalOptions2
        {
            public object requiredApproverCount { get; set; }
            public bool releaseCreatorCanBeApprover { get; set; }
            public bool autoTriggeredAndPreviousEnvironmentApprovedCanBeSkipped { get; set; }
            public bool enforceIdentityRevalidation { get; set; }
            public int timeoutInMinutes { get; set; }
            public string executionOrder { get; set; }
        }

        public class PostDeployApprovals
        {
            public List<Approval2> approvals { get; set; }
            public ApprovalOptions2 approvalOptions { get; set; }
        }

        public class ParallelExecution
        {
            public string parallelExecutionType { get; set; }
        }

        public class DownloadInput
        {
            public List<object> artifactItems { get; set; }
            public string alias { get; set; }
            public string artifactType { get; set; }
            public string artifactDownloadMode { get; set; }
        }

        public class ArtifactsDownloadInput
        {
            public List<DownloadInput> downloadInputs { get; set; }
        }

        public class OverrideInputs
        {
        }

        public class DeploymentInput
        {
            public ParallelExecution parallelExecution { get; set; }
            public bool skipArtifactsDownload { get; set; }
            public ArtifactsDownloadInput artifactsDownloadInput { get; set; }
            public int queueId { get; set; }
            public List<object> demands { get; set; }
            public bool enableAccessToken { get; set; }
            public int timeoutInMinutes { get; set; }
            public int jobCancelTimeoutInMinutes { get; set; }
            public string condition { get; set; }
            public OverrideInputs overrideInputs { get; set; }
        }

        public class Environment2
        {
        }

        public class OverrideInputs2
        {
        }

        public class Inputs
        {
            public string ConnectedServiceNameSelector { get; set; }
            public string ConnectedServiceNameARM { get; set; }
            public string action { get; set; }
            public string applicationName { get; set; }
            public string resourceGroupName { get; set; }
            public string location { get; set; }
            public string securityDomain { get; set; }
            public string organisationalGroup { get; set; }
            public string costCenter { get; set; }
            public string incrementResourceGroupNameIfAlreadyExist { get; set; }
            public string resourceGroupSuffixNumber { get; set; }
            public string ConnectedServiceName { get; set; }
            public string ScriptType { get; set; }
            public string ScriptPath { get; set; }
            public string Inline { get; set; }
            public string ScriptArguments { get; set; }
            public string TargetAzurePs { get; set; }
            public string CustomTargetAzurePs { get; set; }
            public string WebAppKind { get; set; }
            public string WebAppName { get; set; }
            public string DeployToSlotFlag { get; set; }
            public string ResourceGroupName { get; set; }
            public string SlotName { get; set; }
            public string ImageSource { get; set; }
            public string AzureContainerRegistry { get; set; }
            public string AzureContainerRegistryLoginServer { get; set; }
            public string AzureContainerRegistryImage { get; set; }
            public string AzureContainerRegistryTag { get; set; }
            public string DockerRepositoryAccess { get; set; }
            public string RegistryConnectedServiceName { get; set; }
            public string PrivateRegistryImage { get; set; }
            public string PrivateRegistryTag { get; set; }
            public string DockerNamespace { get; set; }
            public string DockerRepository { get; set; }
            public string DockerImageTag { get; set; }
            public string VirtualApplication { get; set; }
            public string Package { get; set; }
            public string BuiltinLinuxPackage { get; set; }
            public string RuntimeStack { get; set; }
            public string StartupCommand { get; set; }
            public string WebAppUri { get; set; }
            public string InlineScript { get; set; }
            public string GenerateWebConfig { get; set; }
            public string WebConfigParameters { get; set; }
            public string AppSettings { get; set; }
            public string ConfigurationSettings { get; set; }
            public string TakeAppOfflineFlag { get; set; }
            public string UseWebDeploy { get; set; }
            public string SetParametersFile { get; set; }
            public string RemoveAdditionalFilesFlag { get; set; }
            public string ExcludeFilesFromAppDataFlag { get; set; }
            public string AdditionalArguments { get; set; }
            public string RenameFilesFlag { get; set; }
            public string XmlTransformation { get; set; }
            public string XmlVariableSubstitution { get; set; }
            public string JSONFiles { get; set; }
        }

        public class WorkflowTask
        {
            public Environment2 environment { get; set; }
            public string taskId { get; set; }
            public string version { get; set; }
            public string name { get; set; }
            public string refName { get; set; }
            public bool enabled { get; set; }
            public bool alwaysRun { get; set; }
            public bool continueOnError { get; set; }
            public int timeoutInMinutes { get; set; }
            public string definitionType { get; set; }
            public OverrideInputs2 overrideInputs { get; set; }
            public string condition { get; set; }
            public Inputs inputs { get; set; }
        }

        public class DeployPhas
        {
            public DeploymentInput deploymentInput { get; set; }
            public int rank { get; set; }
            public string phaseType { get; set; }
            public string name { get; set; }
            public List<WorkflowTask> workflowTasks { get; set; }
        }

        public class EnvironmentOptions
        {
            public string emailNotificationType { get; set; }
            public string emailRecipients { get; set; }
            public bool skipArtifactsDownload { get; set; }
            public int timeoutInMinutes { get; set; }
            public bool enableAccessToken { get; set; }
            public bool publishDeploymentStatus { get; set; }
            public bool badgeEnabled { get; set; }
            public bool autoLinkWorkItems { get; set; }
            public bool pullRequestDeploymentEnabled { get; set; }
        }

        public class Condition
        {
            public string name { get; set; }
            public string conditionType { get; set; }
            public string value { get; set; }
        }

        public class ExecutionPolicy
        {
            public int concurrencyCount { get; set; }
            public int queueDepthCount { get; set; }
        }

        public class Links5
        {
        }

        public class CurrentRelease
        {
            public int id { get; set; }
            public string url { get; set; }
            public Links5 _links { get; set; }
        }

        public class RetentionPolicy
        {
            public int daysToKeep { get; set; }
            public int releasesToKeep { get; set; }
            public bool retainBuild { get; set; }
        }

        public class Options
        {
            public string app { get; set; }
            public string applinux { get; set; }
            public string functionapp { get; set; }
            public string api { get; set; }
            public string mobileapp { get; set; }
            public string Registry { get; set; }
            public string Builtin { get; set; }
        }

        public class Properties
        {
            public string EditableOptions { get; set; }
        }

        public class Input
        {
            public List<object> aliases { get; set; }
            public Options options { get; set; }
            public Properties properties { get; set; }
            public string name { get; set; }
            public string label { get; set; }
            public string defaultValue { get; set; }
            public bool required { get; set; }
            public string type { get; set; }
            public string helpMarkDown { get; set; }
            public string visibleRule { get; set; }
            public string groupName { get; set; }
        }

        public class Parameters
        {
            public string WebAppKind { get; set; }
        }

        public class DataSourceBinding
        {
            public string dataSourceName { get; set; }
            public Parameters parameters { get; set; }
            public string endpointId { get; set; }
            public string target { get; set; }
        }

        public class ProcessParameters
        {
            public List<Input> inputs { get; set; }
            public List<DataSourceBinding> dataSourceBindings { get; set; }
        }

        public class Properties2
        {
        }

        public class PreDeploymentGates
        {
            public int id { get; set; }
            public object gatesOptions { get; set; }
            public List<object> gates { get; set; }
        }

        public class PostDeploymentGates
        {
            public int id { get; set; }
            public object gatesOptions { get; set; }
            public List<object> gates { get; set; }
        }

        public class Environment
        {
            public int id { get; set; }
            public string name { get; set; }
            public int rank { get; set; }
            public Owner owner { get; set; }
            public Variables2 variables { get; set; }
            public List<object> variableGroups { get; set; }
            public PreDeployApprovals preDeployApprovals { get; set; }
            public DeployStep deployStep { get; set; }
            public PostDeployApprovals postDeployApprovals { get; set; }
            public List<DeployPhas> deployPhases { get; set; }
            public EnvironmentOptions environmentOptions { get; set; }
            public List<object> demands { get; set; }
            public List<Condition> conditions { get; set; }
            public ExecutionPolicy executionPolicy { get; set; }
            public List<object> schedules { get; set; }
            public CurrentRelease currentRelease { get; set; }
            public RetentionPolicy retentionPolicy { get; set; }
            public ProcessParameters processParameters { get; set; }
            public Properties2 properties { get; set; }
            public PreDeploymentGates preDeploymentGates { get; set; }
            public PostDeploymentGates postDeploymentGates { get; set; }
            public List<object> environmentTriggers { get; set; }
            public string badgeUrl { get; set; }
        }

        public class ArtifactSourceDefinitionUrl
        {
            public string id { get; set; }
            public string name { get; set; }
        }

        public class DefaultVersionBranch
        {
            public string id { get; set; }
            public string name { get; set; }
        }

        public class DefaultVersionSpecific
        {
            public string id { get; set; }
            public string name { get; set; }
        }

        public class DefaultVersionTags
        {
            public string id { get; set; }
            public string name { get; set; }
        }

        public class DefaultVersionType
        {
            public string id { get; set; }
            public string name { get; set; }
        }

        public class Definition
        {
            public string id { get; set; }
            public string name { get; set; }
        }

        public class Project
        {
            public string id { get; set; }
            public string name { get; set; }
        }

        public class DefinitionReference
        {
            public ArtifactSourceDefinitionUrl artifactSourceDefinitionUrl { get; set; }
            public DefaultVersionBranch defaultVersionBranch { get; set; }
            public DefaultVersionSpecific defaultVersionSpecific { get; set; }
            public DefaultVersionTags defaultVersionTags { get; set; }
            public DefaultVersionType defaultVersionType { get; set; }
            public Definition definition { get; set; }
            public Project project { get; set; }
        }

        public class Artifact
        {
            public string sourceId { get; set; }
            public string type { get; set; }
            public string alias { get; set; }
            public DefinitionReference definitionReference { get; set; }
            public bool isPrimary { get; set; }
            public bool isRetained { get; set; }
        }

        public class Trigger
        {
            public string artifactAlias { get; set; }
            public List<object> triggerConditions { get; set; }
            public string triggerType { get; set; }
        }

        public class PipelineProcess
        {
            public string type { get; set; }
        }

        public class DefinitionCreationSource
        {
            //    public string __invalid_name__$type { get; set; }
            //public string __invalid_name__$value { get; set; }
        }

        public class Properties3
        {
            public DefinitionCreationSource DefinitionCreationSource { get; set; }
        }

        public class Self
        {
            public string href { get; set; }
        }

        public class Web
        {
            public string href { get; set; }
        }

        public class Links6
        {
            public Self self { get; set; }
            public Web web { get; set; }
        }

        public class RootObject
        {
            public string source { get; set; }
            public int revision { get; set; }
            public object description { get; set; }
            public CreatedBy createdBy { get; set; }
            public DateTime createdOn { get; set; }
            public ModifiedBy modifiedBy { get; set; }
            public DateTime modifiedOn { get; set; }
            public bool isDeleted { get; set; }
            public Variables variables { get; set; }
            public List<object> variableGroups { get; set; }
            public List<Environment> environments { get; set; }
            public List<Artifact> artifacts { get; set; }
            public List<Trigger> triggers { get; set; }
            public string releaseNameFormat { get; set; }
            public List<object> tags { get; set; }
            public PipelineProcess pipelineProcess { get; set; }
            public Properties3 properties { get; set; }
            public int id { get; set; }
            public string name { get; set; }
            public string path { get; set; }
            public object projectReference { get; set; }
            public string url { get; set; }
            public Links6 _links { get; set; }
        }
    }

}
