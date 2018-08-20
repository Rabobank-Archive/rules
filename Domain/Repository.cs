namespace Domain
{
    public class Repository
    {
        public string Name { get; set; }

        public bool RequireAMinimumOfReviewers { get; set; }

        public bool HasCodeReviewer { get; set; }

        public bool MasterBranchIsReadOnly { get; set; }
    }
}