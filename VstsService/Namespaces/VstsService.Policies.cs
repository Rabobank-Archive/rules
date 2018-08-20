using System;
using System.Collections.Generic;
using System.Text;

namespace VstsService
{
    namespace Policies
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

        public class Scope
        {
            public string refName { get; set; }
            public string matchKind { get; set; }
            public string repositoryId { get; set; }
        }

        public class Settings
        {
            public int buildDefinitionId { get; set; }
            public bool queueOnSourceUpdateOnly { get; set; }
            public bool manualQueueOnly { get; set; }
            public string displayName { get; set; }
            public double validDuration { get; set; }
            public List<Scope> scope { get; set; }
            public List<string> requiredReviewerIds { get; set; }
            public int? minimumApproverCount { get; set; }
            public bool? creatorVoteCounts { get; set; }
            public bool? allowDownvotes { get; set; }
            public bool? resetOnSourcePush { get; set; }
            public object enforceConsistentCase { get; set; }
            public object rejectDotGit { get; set; }
            public object optimizedByDefault { get; set; }
            public object breadcrumbDays { get; set; }
            public object allowedForkTargets { get; set; }
            public object gvfsOnly { get; set; }
            public object gvfsExemptUsers { get; set; }
            public object gvfsAllowedVersionRanges { get; set; }
            public object detectRenameFalsePositivesByDefault { get; set; }
            public bool? useSquashMerge { get; set; }
        }

        public class Self
        {
            public string href { get; set; }
        }

        public class PolicyType
        {
            public string href { get; set; }
        }

        public class Links2
        {
            public Self self { get; set; }
            public PolicyType policyType { get; set; }
        }

        public class Type
        {
            public string id { get; set; }
            public string url { get; set; }
            public string displayName { get; set; }
        }

        public class Value
        {
            public CreatedBy createdBy { get; set; }
            public DateTime createdDate { get; set; }
            public bool isEnabled { get; set; }
            public bool isBlocking { get; set; }
            public bool isDeleted { get; set; }
            public Settings settings { get; set; }
            public Links2 _links { get; set; }
            public int revision { get; set; }
            public int id { get; set; }
            public string url { get; set; }
            public Type type { get; set; }
        }

        public class RootObject
        {
            public int count { get; set; }
            public List<Value> value { get; set; }
        }
    }
}
