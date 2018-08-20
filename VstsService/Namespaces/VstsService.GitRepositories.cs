using System;
using System.Collections.Generic;
using System.Text;

namespace VstsService
{
    namespace GitRepositories
    {
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

        public class Value
        {
            public string id { get; set; }
            public string name { get; set; }
            public string url { get; set; }
            public Project project { get; set; }
            public string defaultBranch { get; set; }
            public int size { get; set; }
            public string remoteUrl { get; set; }
            public string sshUrl { get; set; }
        }

        public class RootObject
        {
            public List<Value> value { get; set; }
            public int count { get; set; }
        }
    }
}
