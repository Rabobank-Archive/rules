using System;

namespace SecurePipelineScan.VstsService
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message)
        {
        }
    }
}