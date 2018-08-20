using System;
using System.Collections.Generic;
using System.Text;
using Domain;

namespace VstsService
{
    namespace ReleaseDefinitions {
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

        public class Links3
        {
            public Self self { get; set; }
            public Web web { get; set; }
        }

        public class Value
        {
            public string source { get; set; }
            public int revision { get; set; }
            public object description { get; set; }
            public CreatedBy createdBy { get; set; }
            public DateTime createdOn { get; set; }
            public ModifiedBy modifiedBy { get; set; }
            public DateTime modifiedOn { get; set; }
            public bool isDeleted { get; set; }
            public object variableGroups { get; set; }
            public string releaseNameFormat { get; set; }
            public Properties properties { get; set; }
            public int id { get; set; }
            public string name { get; set; }
            public string path { get; set; }
            public object projectReference { get; set; }
            public string url { get; set; }
            public Links3 _links { get; set; }
        }

        public class RootObject
        {
            public int count { get; set; }
            public List<Value> value { get; set; }

            
        }
    }
}
