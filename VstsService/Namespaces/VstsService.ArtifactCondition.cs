using System;
using System.Collections.Generic;
using System.Text;

namespace VstsService
{
    namespace ArtifactCondition
    {
        public class RootObject
        {
            public string sourceBranch { get; set; }
            public List<object> tags { get; set; }
            public bool useBuildDefinitionBranch { get; set; }
            public bool createReleaseOnBuildTagging { get; set; }
        }
    }
}