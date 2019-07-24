
using System;
using System.Collections.Generic;

namespace SecurePipelineScan.VstsService
{
    public class ExtmgmtRequest<TResponse> : VstsRequest<TResponse>
        where TResponse: new()
    {
        public ExtmgmtRequest(string resource) : base(resource)
        {
        }

        public ExtmgmtRequest(string resource, IDictionary<string, object> queryParams) : base(resource, queryParams)
        {
        }

        public override Uri BaseUri(string organization) => new Uri($"https://{organization}.extmgmt.visualstudio.com/");
    }
}