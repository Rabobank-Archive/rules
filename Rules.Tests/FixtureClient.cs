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

        public TResponse Get<TResponse>(IVstsRestRequest<TResponse> request) where TResponse : new()
        {
            return _fixture.Create<TResponse>();
        }

        public TResponse Post<TInput, TResponse>(IVstsPostRequest<TInput, TResponse> request, TInput body) where TResponse : new()
        {
            throw new NotImplementedException();
        }

        public TResponse Put<TResponse>(IVstsRestRequest<TResponse> request, TResponse body) where TResponse : new()
        {
            throw new NotImplementedException();
        }

        public void Delete(IVstsRestRequest request)
        {
            throw new NotImplementedException();
        }
    }
}