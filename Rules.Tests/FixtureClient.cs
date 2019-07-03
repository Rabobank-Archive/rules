using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Response;
using Task = System.Threading.Tasks.Task;

namespace SecurePipelineScan.Rules.Tests
{
    public class FixtureClient : IVstsRestClient
    {
        private readonly IFixture _fixture;

        public FixtureClient(IFixture fixture)
        {
            _fixture = fixture;
        }

        public Task<TResponse> GetAsync<TResponse>(IVstsRequest<TResponse> request) where TResponse : new()
        {
            return Task.FromResult(_fixture.Create<TResponse>());
        }

        public Task<TResponse> GetAsync<TResponse>(string url) where TResponse : new()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TResponse> Get<TResponse>(IVstsRequest<Multiple<TResponse>> request) where TResponse : new()
        {
            return _fixture.CreateMany<TResponse>();
        }

        public Task<TResponse> PostAsync<TInput, TResponse>(IVstsRequest<TInput, TResponse> request, TInput body) where TResponse : new()
        {
            throw new NotImplementedException();
        }

        public Task<TResponse> PutAsync<TInput, TResponse>(IVstsRequest<TInput, TResponse> request, TInput body) where TResponse : new()
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(IVstsRequest request)
        {
            throw new NotImplementedException();
        }
    }
}