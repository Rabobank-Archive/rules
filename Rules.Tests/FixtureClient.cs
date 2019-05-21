using System;
using AutoFixture;
using SecurePipelineScan.VstsService;

namespace SecurePipelineScan.Rules.Tests
{
    public class FixtureClient : IVstsRestClient
    {
        private readonly IFixture _fixture;

        public FixtureClient(IFixture fixture)
        {
            _fixture = fixture;
        }

        public TResponse Get<TResponse>(IVstsRequest<TResponse> request) where TResponse : new()
        {
            return _fixture.Create<TResponse>();
        }

        public TResponse Post<TInput, TResponse>(IVstsRequest<TInput, TResponse> request, TInput body) where TResponse : new()
        {
            throw new NotImplementedException();
        }

        public TResponse Put<TInput, TResponse>(IVstsRequest<TInput, TResponse> request, TInput body) where TResponse : new()
        {
            throw new NotImplementedException();
        }

        public void Delete(IVstsRequest request)
        {
            throw new NotImplementedException();
        }
    }
}