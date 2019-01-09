using System;
using SecurePipelineScan.Rules.Reports;
using SecurePipelineScan.VstsService.Response;

namespace Rules.Reports
{
    public class RepositoryReport
    {
        public RepositoryReport(DateTime date)
        {
            Date = date;
        }

        public string Project { get; set; }
        public string Repository { get; set; }
        public bool HasRequiredReviewerPolicy { get; set; }
        public DateTime Date { get; }
    }
}