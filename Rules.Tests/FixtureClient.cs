using System;
using System.Collections.Generic;
using AutoFixture;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Response;

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

        public IEnumerable<TResponse> Get<TResponse>(IVstsRequest<Multiple<TResponse>> request) where TResponse : new()
        {
            return _fixture.CreateMany<TResponse>();
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