using System;
using System.Collections.Generic;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Newtonsoft.Json.Linq;
using NSubstitute;
using SecurePipelineScan.Rules.Security;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Requests;
using SecurePipelineScan.VstsService.Response;
using Shouldly;
using Xunit;
using Project = SecurePipelineScan.VstsService.Requests.Project;
using Repository = SecurePipelineScan.VstsService.Response.Repository;
using Task = System.Threading.Tasks.Task;
using TaskGroup = SecurePipelineScan.VstsService.Response.TaskGroup;

namespace SecurePipelineScan.Rules.Tests.Security
{
    public class BuildPipelineHasFortifyTaskTests : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;
        private readonly Fixture _fixture = new Fixture { RepeatCount = 1 };
        private const string TaskName = "FortifySCA";

        public BuildPipelineHasFortifyTaskTests(TestConfig config)
        {
            _config = config;
            _fixture.Customize(new AutoNSubstituteCustomization());
        }

        #region [ gui ]

        [Fact]
        [Trait("category", "integration")]
        public async Task EvaluateGuiIntegrationTest()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var project = await client.GetAsync(Project.ProjectById(_config.Project));
            var buildPipeline = await client.GetAsync(Builds.BuildDefinition(project.Id, "2"))
                .ConfigureAwait(false);

            var rule = new BuildPipelineHasFortifyTask(client);
            (await rule.EvaluateAsync(project, buildPipeline).ConfigureAwait(false))
                .GetValueOrDefault().ShouldBeTrue();
        }

        #region [ nested taskgroup ]

        [Fact]
        public async Task GivenPipeline_WhenNestedTaskGroupWithFortifyTask_ThenEvaluatesToTrue()
        {
            _fixture.Customize<BuildProcess>(ctx => ctx
                .With(p => p.Type, 1));
            _fixture.Customize<VstsService.Response.Project>(ctx => ctx
                .With(x => x.Name, "projectA"));
            _fixture.Customize<Repository>(ctx => ctx
                .With(r => r.Url, new Uri("https://projectA.nl")));
            _fixture.Customize<BuildStep>(ctx => ctx
                .With(b => b.Enabled, true));
            _fixture.Customize<BuildTask>(ctx => ctx
                .With(t => t.DefinitionType, "metaTask")
                .With(t => t.Id, "df6aa8e5-82dc-468c-a794-a7990523363d"));

            var fortifyStep = new BuildStep
            {
                Enabled = true,
                Task = new BuildTask
                {
                    Id = "818386e5-c8a5-46c3-822d-954b3c8fb130"
                }
            };

            var taskGroup = new TaskGroup { Tasks = new[] { fortifyStep } };
            var taskGroupResponse = new TaskGroupResponse { Value = new List<TaskGroup> { taskGroup } };

            var buildPipeline = _fixture.Create<BuildDefinition>();
            var project = _fixture.Create<VstsService.Response.Project>();

            var client = Substitute.For<IVstsRestClient>();
            client.GetAsync(Arg.Any<IVstsRequest<TaskGroupResponse>>()).Returns(taskGroupResponse);

            var rule = new BuildPipelineHasFortifyTask(client);
            var result = await rule.EvaluateAsync(project, buildPipeline).ConfigureAwait(false);

            result.ShouldBe(true);
        }

        [Fact]
        public async Task GivenPipeline_WhenNestedTaskGroupWithCircularDependencyAndNoFortifyTask_ThenEvaluatesToFalse()
        {
            _fixture.Customize<BuildProcess>(ctx => ctx
                .With(p => p.Type, 1));
            _fixture.Customize<VstsService.Response.Project>(ctx => ctx
                .With(x => x.Name, "projectA"));
            _fixture.Customize<Repository>(ctx => ctx
                .With(r => r.Url, new Uri("https://projectA.nl")));
            _fixture.Customize<BuildStep>(ctx => ctx
                .With(b => b.Enabled, true));
            _fixture.Customize<BuildTask>(ctx => ctx
                .With(t => t.DefinitionType, "metaTask")
                .With(t => t.Id, "df6aa8e5-82dc-468c-a794-a7990523363d"));

            var circularStep = new BuildStep
            {
                Enabled = true,
                Task = new BuildTask
                {
                    Id = "df6aa8e5-82dc-468c-a794-a7990523363d",
                    DefinitionType = "metaTask"
                }
            };

            var taskGroup = new TaskGroup { Tasks = new[] { circularStep } };
            var taskGroupResponse = new TaskGroupResponse { Value = new List<TaskGroup> { taskGroup } };

            var buildPipeline = _fixture.Create<BuildDefinition>();
            var project = _fixture.Create<VstsService.Response.Project>();

            var client = Substitute.For<IVstsRestClient>();
            client.GetAsync(Arg.Any<IVstsRequest<TaskGroupResponse>>()).Returns(taskGroupResponse);

            var rule = new BuildPipelineHasFortifyTask(client);
            var result = await rule.EvaluateAsync(project, buildPipeline).ConfigureAwait(false);

            result.ShouldBe(false);
        }

        #endregion

        #endregion

        #region [ yaml ]

        [Fact]
        [Trait("category", "integration")]
        public async Task EvaluateYamlIntegrationTest()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var project = await client.GetAsync(Project.ProjectById(_config.Project));
            var buildPipeline = await client.GetAsync(Builds.BuildDefinition(project.Id, "197"))
                .ConfigureAwait(false);

            var rule = new BuildPipelineHasFortifyTask(client);
            (await rule.EvaluateAsync(project, buildPipeline).ConfigureAwait(false))
                .GetValueOrDefault().ShouldBeFalse();
        }

        [Fact(Skip = "Nested YAML Pipeline should first be fixed")]
        [Trait("category", "integration")]
        public async Task EvaluateNestedYamlTemplatesIntegrationTest()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var project = await client.GetAsync(Project.ProjectById(_config.Project));
            var buildPipeline = await client.GetAsync(Builds.BuildDefinition(project.Id, "275"))
                .ConfigureAwait(false);

            var rule = new BuildPipelineHasFortifyTask(client);
            (await rule.EvaluateAsync(project, buildPipeline).ConfigureAwait(false))
                .GetValueOrDefault().ShouldBeTrue();
        }

        [Fact]
        public async Task GivenPipeline_WhenStepsYamlFileWithFortifyTask_ThenEvaluatesToTrue()
        {
            _fixture.Customize<BuildProcess>(ctx => ctx
                .With(p => p.Type, 2));
            _fixture.Customize<VstsService.Response.Project>(ctx => ctx
                .With(x => x.Name, "projectA"));
            _fixture.Customize<Repository>(ctx => ctx
                .With(r => r.Url, new Uri("https://projectA.nl")));

            var gitItem = new JObject
            {
                {"content", $"steps:\r- task: {TaskName}@5"}
            };

            var buildPipeline = _fixture.Create<BuildDefinition>();
            var project = _fixture.Create<VstsService.Response.Project>();

            var client = Substitute.For<IVstsRestClient>();
            client.GetAsync(Arg.Any<IVstsRequest<JObject>>()).Returns(gitItem);

            var rule = new BuildPipelineHasFortifyTask(client);
            var result = await rule.EvaluateAsync(project, buildPipeline).ConfigureAwait(false);

            result.ShouldBe(true);
        }

        [Fact]
        public async Task GivenPipeline_WhenJobsYamlFileWithFortifyTask_ThenEvaluatesToTrue()
        {
            _fixture.Customize<BuildProcess>(ctx => ctx
                .With(p => p.Type, 2));
            _fixture.Customize<VstsService.Response.Project>(ctx => ctx
                .With(x => x.Name, "projectA"));
            _fixture.Customize<Repository>(ctx => ctx
                .With(r => r.Url, new Uri("https://projectA.nl")));

            var gitItem = new JObject
            {
                {
                    "content", $@"
jobs:
- job: JobName
  steps:
  - task: {TaskName}@5"
                }
            };

            var buildPipeline = _fixture.Create<BuildDefinition>();
            var project = _fixture.Create<VstsService.Response.Project>();

            var client = Substitute.For<IVstsRestClient>();
            client.GetAsync(Arg.Any<IVstsRequest<JObject>>()).Returns(gitItem);

            var rule = new BuildPipelineHasFortifyTask(client);
            var result = await rule.EvaluateAsync(project, buildPipeline).ConfigureAwait(false);

            result.ShouldBe(true);
        }

        [Theory]
        [InlineData("Fortify_SCA@5")]
        [InlineData("_FortifySCA@5")]
        [InlineData("FortifySCA_@5")]
        [InlineData("Fortify_SCA")]
        [InlineData("_FortifySCA")]
        [InlineData("FortifySCA_")]
        [InlineData(" ")]
        [InlineData("")]
        [InlineData(null)]
        public async Task GivenPipeline_WhenYamlFileWithoutFortifyTask_ThenEvaluatesToFalse(string fortifyTask)
        {
            _fixture.Customize<BuildProcess>(ctx => ctx
                .With(p => p.Type, 2));
            _fixture.Customize<VstsService.Response.Project>(ctx => ctx
                .With(x => x.Name, "projectA"));
            _fixture.Customize<Repository>(ctx => ctx
                .With(r => r.Url, new Uri("https://projectA.nl")));

            var gitItem = new JObject
            {
                {"content", $"steps:\r- task: {fortifyTask}"}
            };

            var buildPipeline = _fixture.Create<BuildDefinition>();
            var project = _fixture.Create<VstsService.Response.Project>();

            var client = Substitute.For<IVstsRestClient>();
            client.GetAsync(Arg.Any<IVstsRequest<JObject>>()).Returns(gitItem);

            var rule = new BuildPipelineHasFortifyTask(client);
            var result = await rule.EvaluateAsync(project, buildPipeline).ConfigureAwait(false);

            result.ShouldBe(false);
        }

        #region [ nested steps templates in same repo ]

        [Fact]
        public async Task GivenPipeline_WhenNestedStepsYamlTemplateInSameRepoWithFortifyTask_ThenEvaluatesToTrue()
        {
            // arrange
            _fixture.Customize<BuildProcess>(ctx => ctx
                .With(p => p.Type, 2)
                .With(p => p.YamlFilename, "azure-pipelines.yml"));
            _fixture.Customize<VstsService.Response.Project>(ctx => ctx
                .With(x => x.Name, "projectA"));
            _fixture.Customize<Repository>(ctx => ctx
                .With(r => r.Url, new Uri("https://projectA.nl")));

            var azurePipelineGitItem = new JObject
            {
                {"content", "steps:\r- template: steps-template.yml"}
            };

            var stepsTemplateGitItem = new JObject
            {
                {"content", $"steps:\r- task: {TaskName}@5"}
            };

            var client = Substitute.For<IVstsRestClient>();
            client.GetAsync(Arg.Is<IVstsRequest<JObject>>(
                    r => r.QueryParams["path"].ToString() == "azure-pipelines.yml"))
                .Returns(azurePipelineGitItem);
            client.GetAsync(Arg.Is<IVstsRequest<JObject>>(
                    r => r.QueryParams["path"].ToString() == "steps-template.yml"))
                .Returns(stepsTemplateGitItem);

            var buildPipeline = _fixture.Create<BuildDefinition>();
            var project = _fixture.Create<VstsService.Response.Project>();

            var rule = new BuildPipelineHasFortifyTask(client);

            // act
            var result = await rule.EvaluateAsync(project, buildPipeline).ConfigureAwait(false);

            // assert
            result.ShouldBe(true);
        }

        [Fact]
        public async Task GivenPipeline_WhenNestedJobsYamlTemplateInSameRepoWithFortifyTask_ThenEvaluatesToTrue()
        {
            // arrange
            _fixture.Customize<BuildProcess>(ctx => ctx
                .With(p => p.Type, 2)
                .With(p => p.YamlFilename, "azure-pipelines.yml"));
            _fixture.Customize<VstsService.Response.Project>(ctx => ctx
                .With(x => x.Name, "projectA"));
            _fixture.Customize<Repository>(ctx => ctx
                .With(r => r.Url, new Uri("https://projectA.nl")));

            var azurePipelineGitItem = new JObject
            {
                {
                    "content", @"
jobs:
- template: jobs-template.yml"
                }
            };

            var jobsTemplateGitItem = new JObject
            {
                {
                    "content", $@"
jobs:
- job:
  steps:
  - task: {TaskName}@5"
                }
            };

            var client = Substitute.For<IVstsRestClient>();
            client.GetAsync(Arg.Is<IVstsRequest<JObject>>(
                    r => r.QueryParams["path"].ToString() == "azure-pipelines.yml"))
                .Returns(azurePipelineGitItem);
            client.GetAsync(Arg.Is<IVstsRequest<JObject>>(
                    r => r.QueryParams["path"].ToString() == "jobs-template.yml"))
                .Returns(jobsTemplateGitItem);

            var buildPipeline = _fixture.Create<BuildDefinition>();
            var project = _fixture.Create<VstsService.Response.Project>();

            var rule = new BuildPipelineHasFortifyTask(client);

            // act
            var result = await rule.EvaluateAsync(project, buildPipeline).ConfigureAwait(false);

            // assert
            result.ShouldBe(true);
        }

        [Fact]
        public async Task GivenPipeline_WhenMultipleNestedStepsYamlTemplatesInSameRepoWithFortifyTask_ThenEvaluatesToTrue()
        {
            // arrange
            _fixture.Customize<BuildProcess>(ctx => ctx
                .With(p => p.Type, 2)
                .With(p => p.YamlFilename, "azure-pipelines.yml"));
            _fixture.Customize<VstsService.Response.Project>(ctx => ctx
                .With(x => x.Name, "projectA"));
            _fixture.Customize<Repository>(ctx => ctx
                .With(r => r.Url, new Uri("https://projectA.nl")));

            var azurePipelineGitItem = new JObject
            {
                {"content", "steps:\r- template: YamlTemplates/steps-template1.yml"}
            };

            var stepsTemplate1GitItem = new JObject
            {
                // template 2 and 3 are in the same folder as template 1 so from template
                // 1 they should be referenced without the path prefix.
                {"content", "steps:\r- template: steps-template2.yml"}
            };

            var stepsTemplate2GitItem = new JObject
            {
                {"content", "steps:\r- template: steps-template3.yml"}
            };

            var stepsTemplate3GitItem = new JObject
            {
                {"content", $"steps:\r- task: {TaskName}@5"}
            };

            var client = Substitute.For<IVstsRestClient>();
            client.GetAsync(Arg.Is<IVstsRequest<JObject>>(
                    r => r.QueryParams["path"].ToString() == "azure-pipelines.yml"))
                .Returns(azurePipelineGitItem);
            client.GetAsync(Arg.Is<IVstsRequest<JObject>>(
                    r => r.QueryParams["path"].ToString() == "YamlTemplates/steps-template1.yml"))
                .Returns(stepsTemplate1GitItem);
            client.GetAsync(Arg.Is<IVstsRequest<JObject>>(
                    r => r.QueryParams["path"].ToString() == "YamlTemplates/steps-template2.yml"))
                .Returns(stepsTemplate2GitItem);
            client.GetAsync(Arg.Is<IVstsRequest<JObject>>(
                    r => r.QueryParams["path"].ToString() == "YamlTemplates/steps-template3.yml"))
                .Returns(stepsTemplate3GitItem);

            var buildPipeline = _fixture.Create<BuildDefinition>();
            var project = _fixture.Create<VstsService.Response.Project>();

            var rule = new BuildPipelineHasFortifyTask(client);

            // act
            var result = await rule.EvaluateAsync(project, buildPipeline).ConfigureAwait(false);

            // assert
            result.ShouldBe(true);
        }

        [Fact]
        public async Task GivenPipeline_WhenNestedJobsWithNestedStepsYamlTemplatesWithFortifyTask_ThenEvaluatesToTrue()
        {
            // arrange
            const string projectId = "Idd48fb56b-b2c0-454a-9d6b-4cd7175c8665";
            const string repoId = "Id47fefefe-013a-465b-8977-bfecc355bee0";

            _fixture.Customize<BuildProcess>(ctx => ctx
                .With(p => p.Type, 2)
                .With(p => p.YamlFilename, "azure-pipelines.yml"));
            _fixture.Customize<VstsService.Response.Project>(ctx => ctx
                .With(x => x.Id, projectId)
                .With(x => x.Name, "projectA"));
            _fixture.Customize<Repository>(ctx => ctx
                .With(r => r.Id, "Id47fefefe-013a-465b-8977-bfecc355bee0")
                .With(r => r.Url, new Uri("https://projectA.nl")));

            var client = Substitute.For<IVstsRestClient>();

            // If azure-pipelines.yml is requested in the current project and repo context we will return a yaml
            // that references a jobs template in a subfolder of the current repo
            client.GetAsync(Arg.Is<IVstsRequest<JObject>>(
                    r => r.QueryParams["path"].ToString() == "azure-pipelines.yml" &&
                         r.Resource.Contains($"/{projectId}/_apis/git/repositories/{repoId}/items")))
                .Returns(new JObject
                {
                    {
                        "content", @"
jobs:
  - template: YamlTemplates1/jobs-template.yml"
                    }
                });

            // The jobs template references a steps template in a subfolder of the current folder in the current repo
            client.GetAsync(Arg.Is<IVstsRequest<JObject>>(
                    r => r.QueryParams["path"].ToString() == "YamlTemplates1/jobs-template.yml" &&
                         r.Resource.Contains($"/{projectId}/_apis/git/repositories/{repoId}/items")))
                .Returns(new JObject
                {
                    // template 2 and 3 are in the same folder as template 1 so from template
                    // 1 they should be referenced without the path prefix.
                    {
                        "content", @"
jobs:
- job: JobName
  steps:
  - template: SubFolder/steps-template1.yml"
                    }
                });

            // This steps template references a yaml steps template in a subfolder in another repo. If we reference to
            // another repo the path should start from the root again.
            client.GetAsync(Arg.Is<IVstsRequest<JObject>>(
                    r => r.QueryParams["path"].ToString() == "YamlTemplates1/SubFolder/steps-template1.yml" &&
                         r.Resource.Contains($"/{projectId}/_apis/git/repositories/{repoId}/items")))
                .Returns(new JObject
                {
                    {
                        "content", @"
resources:
  repositories:
    - repository: shared
      type: git
      name: project/repo

steps:
- template: YamlTemplates2/steps-template2.yml@shared"
                    }
                });

            // This steps template references a yaml in the same folder
            client.GetAsync(Arg.Is<IVstsRequest<JObject>>(
                    r => r.QueryParams["path"].ToString() == "YamlTemplates2/steps-template2.yml" &&
                         r.Resource.Contains("/project/_apis/git/repositories/repo/items")))
                .Returns(new JObject
                {
                    {
                        "content", @"
steps:
- template: steps-template3.yml"
                    }
                });

            client.GetAsync(Arg.Is<IVstsRequest<JObject>>(
                    r => r.QueryParams["path"].ToString() == "YamlTemplates2/steps-template3.yml" &&
                         r.Resource.Contains("/project/_apis/git/repositories/repo/items")))
                .Returns(new JObject
                {
                    {
                        "content", $@"
steps:
- task: {TaskName}@5"
                    }
                });

            var buildPipeline = _fixture.Create<BuildDefinition>();
            var project = _fixture.Create<VstsService.Response.Project>();
            var rule = new BuildPipelineHasFortifyTask(client);

            // act
            var result = await rule.EvaluateAsync(project, buildPipeline).ConfigureAwait(false);

            // assert
            result.ShouldBe(true);
        }

        [Fact]
        public async Task GivenPipeline_WhenTooManyNestedStepsYamlTemplateInSameRepoLevelsWithFortifyTask_ThenShouldThrowException()
        {
            // arrange
            _fixture.Customize<BuildProcess>(ctx => ctx
                .With(p => p.Type, 2)
                .With(p => p.YamlFilename, "azure-pipelines.yml"));
            _fixture.Customize<VstsService.Response.Project>(ctx => ctx
                .With(x => x.Name, "projectA"));
            _fixture.Customize<Repository>(ctx => ctx
                .With(r => r.Url, new Uri("https://projectA.nl")));

            var azurePipelineGitItem = new JObject
            {
                {"content", "steps:\r- template: steps-template1.yml"}
            };

            var stepsTemplate1GitItem = new JObject
            {
                {"content", "steps:\r- template: steps-template2.yml"}
            };

            var stepsTemplate2GitItem = new JObject
            {
                {"content", "steps:\r- template: steps-template3.yml"}
            };

            var stepsTemplate3GitItem = new JObject
            {
                {"content", "steps:\r- template: steps-template4.yml"}
            };

            var stepsTemplate4GitItem = new JObject
            {
                {"content", "steps:\r- template: steps-template5.yml"}
            };

            var stepsTemplate5GitItem = new JObject
            {
                {"content", "steps:\r- template: steps-template6.yml"}
            };

            var stepsTemplate6GitItem = new JObject
            {
                {"content", $"steps:\r- task: {TaskName}@5"}
            };

            var client = Substitute.For<IVstsRestClient>();
            client.GetAsync(Arg.Is<IVstsRequest<JObject>>(
                    r => r.QueryParams["path"].ToString() == "azure-pipelines.yml"))
                .Returns(azurePipelineGitItem);
            client.GetAsync(Arg.Is<IVstsRequest<JObject>>(
                    r => r.QueryParams["path"].ToString() == "steps-template1.yml"))
                .Returns(stepsTemplate1GitItem);
            client.GetAsync(Arg.Is<IVstsRequest<JObject>>(
                    r => r.QueryParams["path"].ToString() == "steps-template2.yml"))
                .Returns(stepsTemplate2GitItem);
            client.GetAsync(Arg.Is<IVstsRequest<JObject>>(
                    r => r.QueryParams["path"].ToString() == "steps-template3.yml"))
                .Returns(stepsTemplate3GitItem);
            client.GetAsync(Arg.Is<IVstsRequest<JObject>>(
                    r => r.QueryParams["path"].ToString() == "steps-template4.yml"))
                .Returns(stepsTemplate4GitItem);
            client.GetAsync(Arg.Is<IVstsRequest<JObject>>(
                    r => r.QueryParams["path"].ToString() == "steps-template5.yml"))
                .Returns(stepsTemplate5GitItem);
            client.GetAsync(Arg.Is<IVstsRequest<JObject>>(
                    r => r.QueryParams["path"].ToString() == "steps-template6.yml"))
                .Returns(stepsTemplate6GitItem);

            var buildPipeline = _fixture.Create<BuildDefinition>();
            var project = _fixture.Create<VstsService.Response.Project>();

            var rule = new BuildPipelineHasFortifyTask(client);

            // act
            var exception = await Assert
                .ThrowsAsync<InvalidOperationException>(() => rule.EvaluateAsync(project, buildPipeline))
                .ConfigureAwait(false);

            // assert
            exception.Message.ShouldBe("template nesting level overflow");
        }

        #endregion

        #region [ nested steps templates in external repo ]

        [Theory]
        [InlineData("steps-template.yml@shared", "shared", "git", "project/repo", true, null)]
        [InlineData("steps-template.yml@shared", "shared", "github", "project/repo", false, null)]
        [InlineData("steps-template.yml@shaarredd", "shared", "git", "project/repo", true, "repo alias not found")]
        [InlineData("steps-template.yml-shared", "shared", "git", "project/repo", false, null)]
        [InlineData("steps-template.yml@shared@1", "shared", "git", "project/repo", true,
            "yaml name with repo reference should have exactly two segments")]
        [InlineData("steps-template.yml@shared", "shared", "git", "project/repo/subRepo", true,
            "repo name should have exactly two segments")]
        [InlineData("steps-template.yml@shared", "shared", "git", "project1/repo", false, null)]
        [InlineData("steps-template.yml@shared", "shared", "github", "project1/repo", false, null)]
        public async Task GivenPipeline_WhenNestedStepsYamlTemplateInExternalRepoWithFortifyTask_ThenEvaluatesToTrue(
            string stepsTemplate, string repoAlias, string repoType, string repoName, bool expectedResult,
            string exceptionMsg)
        {
            // arrange
            _fixture.Customize<BuildProcess>(ctx => ctx
                .With(p => p.Type, 2)
                .With(p => p.YamlFilename, "azure-pipelines.yml"));
            _fixture.Customize<VstsService.Response.Project>(ctx => ctx
                .With(x => x.Name, "projectA"));
            _fixture.Customize<Repository>(ctx => ctx
                .With(r => r.Url, new Uri("https://projectA.nl")));

            var azurePipelineGitItem = new JObject
            {
                {
                    "content", $@"
resources:
  repositories:
    - repository: {repoAlias}
      type: {repoType}
      name: {repoName}

steps:
  - template: {stepsTemplate}"
                }
            };

            var stepsTemplateGitItem = new JObject
            {
                {"content", $"steps:\r- task: {TaskName}@5"}
            };

            var client = Substitute.For<IVstsRestClient>();
            client.GetAsync(Arg.Is<IVstsRequest<JObject>>(
                    r => r.QueryParams["path"].ToString() == "azure-pipelines.yml"))
                .Returns(azurePipelineGitItem);
            client.GetAsync(Arg.Is<IVstsRequest<JObject>>(
                    r => r.QueryParams["path"].ToString() == "steps-template.yml" &&
                         r.Resource.Contains("/project/_apis/git/repositories/repo/items")))
                .Returns(stepsTemplateGitItem);

            var buildPipeline = _fixture.Create<BuildDefinition>();
            var project = _fixture.Create<VstsService.Response.Project>();

            var rule = new BuildPipelineHasFortifyTask(client);

            if (exceptionMsg == null)
            {
                // act
                var result = await rule.EvaluateAsync(project, buildPipeline).ConfigureAwait(false);

                // assert
                result.ShouldBe(expectedResult);
            }
            else
            {
                //act
                var exception = await Assert
                    .ThrowsAsync<InvalidOperationException>(() => rule.EvaluateAsync(project, buildPipeline))
                    .ConfigureAwait(false);

                // assert
                exception.Message.ShouldBe(exceptionMsg);
            }
        }

        #endregion

        #endregion
    }
}