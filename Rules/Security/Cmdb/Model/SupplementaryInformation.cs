﻿using System.Diagnostics.CodeAnalysis;

namespace SecurePipelineScan.Rules.Security.Cmdb.Model
{
    [ExcludeFromCodeCoverage]
    public class SupplementaryInformation
    {
        public string Organization { get; set; }
        public string Project { get; set; }
        public string Pipeline { get; set; }
        public string Stage { get; set; }
    }
}