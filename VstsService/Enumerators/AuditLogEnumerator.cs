using System.Collections.Generic;
using Flurl.Http;
using SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.VstsService.Enumerators
{
    internal class AuditLogEnumerator : IVstsRequestEnumerator<AuditLogEntry>
    {
        public IEnumerable<AuditLogEntry> Enumerate(IFlurlRequest request)
        {
            var more = true;
            while (more)
            {
                var result = request.GetJsonAsync<AuditLogEntries>().ConfigureAwait(false).GetAwaiter().GetResult();
                foreach (var entry in result.DecoratedAuditLogEntries)
                {
                    yield return entry;
                }

                request.SetQueryParam("continuationToken", result.ContinuationToken);
                more = result.ContinuationToken != null;
            }
        }
    }
}