using System;
using System.Collections.Generic;
using SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.VstsService
{
    public static class AuditExtensions
    {
        public static IEnumerable<AuditLogEntry> Get(this IVstsRestClient client, IVstsRequest<AuditLogEntries> request)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));

            return GetInternal(client, request);
        }

        private static IEnumerable<AuditLogEntry> GetInternal(this IVstsRestClient client, IVstsRequest<AuditLogEntries> request)
        {
            var more = true;
            while (more)
            {
                var result = client.GetAsync(request).ConfigureAwait(false).GetAwaiter().GetResult();
                foreach (var entry in result.DecoratedAuditLogEntries)
                {
                    yield return entry;
                }

                if (request != null)
                    request.QueryParams["continuationToken"] = result.ContinuationToken;
                more = result.ContinuationToken != null;
            }
        }
    }
}