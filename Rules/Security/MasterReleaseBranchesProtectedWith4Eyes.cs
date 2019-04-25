using SecurePipelineScan.VstsService;

namespace SecurePipelineScan.Rules.Security
{
    public class MasterReleaseBranchesProtectedWith4Eyes : IRepositoryRule
    {
        
        private const string DeleteRepository = "Delete repository";
        private readonly IVstsRestClient _client;
        
        public MasterReleaseBranchesProtectedWith4Eyes(IVstsRestClient client)
        {
            _client = client;
        }

        public string Description => "iets met 4 eyes";
        
        public bool Evaluate(string project, string repository)
        {
            throw new System.NotImplementedException();
        }
    }
}