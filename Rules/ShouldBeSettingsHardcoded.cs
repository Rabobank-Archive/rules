using System;

namespace SecurePipelineScan.Rules
{
    public class ShouldBeSettingsHardcoded
    {
        public string ShouldBeHardcoded { get; private set; }

        public ShouldBeSettingsHardcoded()
        {
            ShouldBeHardcoded = 
         @"{""projectName"": """",
            ""projectId"": """",
            ""GlobalPermissions"": [
              {
                ""applicationGroupName"": ""Build Administrators"",
                ""permissions"": [
                  {
                    ""permissionBit"": 1048576,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Bypass rules on work item updates"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 8388608,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Change process of team project."",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 2,
                    ""namespaceId"": ""bb50f182-8e5e-40b8-bc21-e8752a1e7ae2"",
                    ""displayName"": ""Create tag definition"",
                    ""permissionId"": 1
                  },
                  {
                    ""permissionBit"": 8,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Create test runs"",
                    ""permissionId"": 1
                  },
                  {
                    ""permissionBit"": 8192,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Delete and restore work items"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 4,
                    ""namespaceId"": ""d34d3680-dfe5-4cc6-a949-7d9c68f73cba"",
                    ""displayName"": ""Delete shared Analytics views"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 4,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Delete team project"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 256,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Delete test runs"",
                    ""permissionId"": 1
                  },
                  {
                    ""permissionBit"": 2,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Edit project-level information"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 2,
                    ""namespaceId"": ""d34d3680-dfe5-4cc6-a949-7d9c68f73cba"",
                    ""displayName"": ""Edit shared Analytics views"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 131072,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Manage project properties"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 4096,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Manage test configurations"",
                    ""permissionId"": 1
                  },
                  {
                    ""permissionBit"": 2048,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Manage test environments"",
                    ""permissionId"": 1
                  },
                  {
                    ""permissionBit"": 16384,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Move work items out of this project"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 32768,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Permanently delete work items"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 65536,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Rename team project"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 2097152,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Suppress notifications for work item updates"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 4194304,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Update project visibility"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 1,
                    ""namespaceId"": ""58450c49-b02d-465a-ab12-59ae512d6531"",
                    ""displayName"": ""View analytics"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 1,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""View project-level information"",
                    ""permissionId"": 1
                  },
                  {
                    ""permissionBit"": 512,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""View test runs"",
                    ""permissionId"": 1
                  }
                ]
              },
              {
                ""applicationGroupName"": ""Contributors"",
                ""permissions"": [
                  {
                    ""permissionBit"": 1048576,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Bypass rules on work item updates"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 8388608,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Change process of team project."",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 2,
                    ""namespaceId"": ""bb50f182-8e5e-40b8-bc21-e8752a1e7ae2"",
                    ""displayName"": ""Create tag definition"",
                    ""permissionId"": 1
                  },
                  {
                    ""permissionBit"": 8,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Create test runs"",
                    ""permissionId"": 1
                  },
                  {
                    ""permissionBit"": 8192,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Delete and restore work items"",
                    ""permissionId"": 1
                  },
                  {
                    ""permissionBit"": 4,
                    ""namespaceId"": ""d34d3680-dfe5-4cc6-a949-7d9c68f73cba"",
                    ""displayName"": ""Delete shared Analytics views"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 4,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Delete team project"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 256,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Delete test runs"",
                    ""permissionId"": 1
                  },
                  {
                    ""permissionBit"": 2,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Edit project-level information"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 2,
                    ""namespaceId"": ""d34d3680-dfe5-4cc6-a949-7d9c68f73cba"",
                    ""displayName"": ""Edit shared Analytics views"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 131072,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Manage project properties"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 4096,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Manage test configurations"",
                    ""permissionId"": 1
                  },
                  {
                    ""permissionBit"": 2048,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Manage test environments"",
                    ""permissionId"": 1
                  },
                  {
                    ""permissionBit"": 16384,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Move work items out of this project"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 32768,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Permanently delete work items"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 65536,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Rename team project"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 2097152,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Suppress notifications for work item updates"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 4194304,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Update project visibility"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 1,
                    ""namespaceId"": ""58450c49-b02d-465a-ab12-59ae512d6531"",
                    ""displayName"": ""View analytics"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 1,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""View project-level information"",
                    ""permissionId"": 1
                  },
                  {
                    ""permissionBit"": 512,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""View test runs"",
                    ""permissionId"": 1
                  }
                ]
              },
              {
                ""applicationGroupName"": ""Production Environment Owners"",
                ""permissions"": [
                  {
                    ""permissionBit"": 1048576,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Bypass rules on work item updates"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 8388608,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Change process of team project."",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 2,
                    ""namespaceId"": ""bb50f182-8e5e-40b8-bc21-e8752a1e7ae2"",
                    ""displayName"": ""Create tag definition"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 8,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Create test runs"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 8192,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Delete and restore work items"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 4,
                    ""namespaceId"": ""d34d3680-dfe5-4cc6-a949-7d9c68f73cba"",
                    ""displayName"": ""Delete shared Analytics views"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 4,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Delete team project"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 256,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Delete test runs"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 2,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Edit project-level information"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 2,
                    ""namespaceId"": ""d34d3680-dfe5-4cc6-a949-7d9c68f73cba"",
                    ""displayName"": ""Edit shared Analytics views"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 131072,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Manage project properties"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 4096,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Manage test configurations"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 2048,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Manage test environments"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 16384,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Move work items out of this project"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 32768,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Permanently delete work items"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 65536,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Rename team project"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 2097152,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Suppress notifications for work item updates"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 4194304,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Update project visibility"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 1,
                    ""namespaceId"": ""58450c49-b02d-465a-ab12-59ae512d6531"",
                    ""displayName"": ""View analytics"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 1,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""View project-level information"",
                    ""permissionId"": 1
                  },
                  {
                    ""permissionBit"": 512,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""View test runs"",
                    ""permissionId"": 0
                  }
                ]
              },
              {
                ""applicationGroupName"": ""Project Administrators"",
                ""permissions"": [
                  {
                    ""permissionBit"": 1048576,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Bypass rules on work item updates"",
                    ""permissionId"": 1
                  },
                  {
                    ""permissionBit"": 8388608,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Change process of team project."",
                    ""permissionId"": 1
                  },
                  {
                    ""permissionBit"": 2,
                    ""namespaceId"": ""bb50f182-8e5e-40b8-bc21-e8752a1e7ae2"",
                    ""displayName"": ""Create tag definition"",
                    ""permissionId"": 1
                  },
                  {
                    ""permissionBit"": 8,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Create test runs"",
                    ""permissionId"": 1
                  },
                  {
                    ""permissionBit"": 8192,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Delete and restore work items"",
                    ""permissionId"": 1
                  },
                  {
                    ""permissionBit"": 4,
                    ""namespaceId"": ""d34d3680-dfe5-4cc6-a949-7d9c68f73cba"",
                    ""displayName"": ""Delete shared Analytics views"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 4,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Delete team project"",
                    ""permissionId"": 1
                  },
                  {
                    ""permissionBit"": 256,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Delete test runs"",
                    ""permissionId"": 1
                  },
                  {
                    ""permissionBit"": 2,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Edit project-level information"",
                    ""permissionId"": 1
                  },
                  {
                    ""permissionBit"": 2,
                    ""namespaceId"": ""d34d3680-dfe5-4cc6-a949-7d9c68f73cba"",
                    ""displayName"": ""Edit shared Analytics views"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 131072,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Manage project properties"",
                    ""permissionId"": 1
                  },
                  {
                    ""permissionBit"": 4096,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Manage test configurations"",
                    ""permissionId"": 1
                  },
                  {
                    ""permissionBit"": 2048,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Manage test environments"",
                    ""permissionId"": 1
                  },
                  {
                    ""permissionBit"": 16384,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Move work items out of this project"",
                    ""permissionId"": 1
                  },
                  {
                    ""permissionBit"": 32768,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Permanently delete work items"",
                    ""permissionId"": 1
                  },
                  {
                    ""permissionBit"": 65536,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Rename team project"",
                    ""permissionId"": 1
                  },
                  {
                    ""permissionBit"": 2097152,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Suppress notifications for work item updates"",
                    ""permissionId"": 1
                  },
                  {
                    ""permissionBit"": 4194304,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Update project visibility"",
                    ""permissionId"": 1
                  },
                  {
                    ""permissionBit"": 1,
                    ""namespaceId"": ""58450c49-b02d-465a-ab12-59ae512d6531"",
                    ""displayName"": ""View analytics"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 1,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""View project-level information"",
                    ""permissionId"": 1
                  },
                  {
                    ""permissionBit"": 512,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""View test runs"",
                    ""permissionId"": 0
                  }
                ]
              },
              {
                ""applicationGroupName"": ""Rabobank Project Administrators"",
                ""permissions"": [
                  {
                    ""permissionBit"": 1048576,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Bypass rules on work item updates"",
                    ""permissionId"": 3
                  },
                  {
                    ""permissionBit"": 8388608,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Change process of team project."",
                    ""permissionId"": 3
                  },
                  {
                    ""permissionBit"": 2,
                    ""namespaceId"": ""bb50f182-8e5e-40b8-bc21-e8752a1e7ae2"",
                    ""displayName"": ""Create tag definition"",
                    ""permissionId"": 3
                  },
                  {
                    ""permissionBit"": 8,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Create test runs"",
                    ""permissionId"": 3
                  },
                  {
                    ""permissionBit"": 8192,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Delete and restore work items"",
                    ""permissionId"": 3
                  },
                  {
                    ""permissionBit"": 4,
                    ""namespaceId"": ""d34d3680-dfe5-4cc6-a949-7d9c68f73cba"",
                    ""displayName"": ""Delete shared Analytics views"",
                    ""permissionId"": 3
                  },
                  {
                    ""permissionBit"": 4,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Delete team project"",
                    ""permissionId"": 2
                  },
                  {
                    ""permissionBit"": 256,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Delete test runs"",
                    ""permissionId"": 3
                  },
                  {
                    ""permissionBit"": 2,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Edit project-level information"",
                    ""permissionId"": 3
                  },
                  {
                    ""permissionBit"": 2,
                    ""namespaceId"": ""d34d3680-dfe5-4cc6-a949-7d9c68f73cba"",
                    ""displayName"": ""Edit shared Analytics views"",
                    ""permissionId"": 3
                  },
                  {
                    ""permissionBit"": 131072,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Manage project properties"",
                    ""permissionId"": 2
                  },
                  {
                    ""permissionBit"": 4096,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Manage test configurations"",
                    ""permissionId"": 3
                  },
                  {
                    ""permissionBit"": 2048,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Manage test environments"",
                    ""permissionId"": 3
                  },
                  {
                    ""permissionBit"": 16384,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Move work items out of this project"",
                    ""permissionId"": 3
                  },
                  {
                    ""permissionBit"": 32768,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Permanently delete work items"",
                    ""permissionId"": 2
                  },
                  {
                    ""permissionBit"": 65536,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Rename team project"",
                    ""permissionId"": 3
                  },
                  {
                    ""permissionBit"": 2097152,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Suppress notifications for work item updates"",
                    ""permissionId"": 3
                  },
                  {
                    ""permissionBit"": 4194304,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Update project visibility"",
                    ""permissionId"": 3
                  },
                  {
                    ""permissionBit"": 1,
                    ""namespaceId"": ""58450c49-b02d-465a-ab12-59ae512d6531"",
                    ""displayName"": ""View analytics"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 1,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""View project-level information"",
                    ""permissionId"": 1
                  },
                  {
                    ""permissionBit"": 512,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""View test runs"",
                    ""permissionId"": 3
                  }
                ]
              },
              {
                ""applicationGroupName"": ""Readers"",
                ""permissions"": [
                  {
                    ""permissionBit"": 1048576,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Bypass rules on work item updates"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 8388608,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Change process of team project."",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 2,
                    ""namespaceId"": ""bb50f182-8e5e-40b8-bc21-e8752a1e7ae2"",
                    ""displayName"": ""Create tag definition"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 8,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Create test runs"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 8192,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Delete and restore work items"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 4,
                    ""namespaceId"": ""d34d3680-dfe5-4cc6-a949-7d9c68f73cba"",
                    ""displayName"": ""Delete shared Analytics views"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 4,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Delete team project"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 256,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Delete test runs"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 2,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Edit project-level information"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 2,
                    ""namespaceId"": ""d34d3680-dfe5-4cc6-a949-7d9c68f73cba"",
                    ""displayName"": ""Edit shared Analytics views"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 131072,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Manage project properties"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 4096,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Manage test configurations"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 2048,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Manage test environments"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 16384,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Move work items out of this project"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 32768,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Permanently delete work items"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 65536,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Rename team project"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 2097152,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Suppress notifications for work item updates"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 4194304,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Update project visibility"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 1,
                    ""namespaceId"": ""58450c49-b02d-465a-ab12-59ae512d6531"",
                    ""displayName"": ""View analytics"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 1,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""View project-level information"",
                    ""permissionId"": 1
                  },
                  {
                    ""permissionBit"": 512,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""View test runs"",
                    ""permissionId"": 1
                  }
                ]
              },
              {
                ""applicationGroupName"": ""Release Administrators"",
                ""permissions"": [
                  {
                    ""permissionBit"": 1048576,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Bypass rules on work item updates"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 8388608,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Change process of team project."",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 2,
                    ""namespaceId"": ""bb50f182-8e5e-40b8-bc21-e8752a1e7ae2"",
                    ""displayName"": ""Create tag definition"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 8,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Create test runs"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 8192,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Delete and restore work items"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 4,
                    ""namespaceId"": ""d34d3680-dfe5-4cc6-a949-7d9c68f73cba"",
                    ""displayName"": ""Delete shared Analytics views"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 4,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Delete team project"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 256,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Delete test runs"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 2,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Edit project-level information"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 2,
                    ""namespaceId"": ""d34d3680-dfe5-4cc6-a949-7d9c68f73cba"",
                    ""displayName"": ""Edit shared Analytics views"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 131072,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Manage project properties"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 4096,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Manage test configurations"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 2048,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Manage test environments"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 16384,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Move work items out of this project"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 32768,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Permanently delete work items"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 65536,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Rename team project"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 2097152,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Suppress notifications for work item updates"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 4194304,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""Update project visibility"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 1,
                    ""namespaceId"": ""58450c49-b02d-465a-ab12-59ae512d6531"",
                    ""displayName"": ""View analytics"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 1,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""View project-level information"",
                    ""permissionId"": 0
                  },
                  {
                    ""permissionBit"": 512,
                    ""namespaceId"": ""52d39943-cb85-4d7f-8fa8-c6baac873819"",
                    ""displayName"": ""View test runs"",
                    ""permissionId"": 0
                  }
                ]
              }
            ]
          }";
        }
    }
}