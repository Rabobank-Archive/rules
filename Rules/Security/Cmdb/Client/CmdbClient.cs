﻿using Flurl;
using Flurl.Http;
using SecurePipelineScan.Rules.Security.Cmdb.Model;
using System.Web;
using System.Linq;
using System.Threading.Tasks;

namespace SecurePipelineScan.Rules.Security.Cmdb.Client
{
    public class CmdbClient : ICmdbClient
    {
        public CmdbClientConfig Config { get; }

        public CmdbClient(CmdbClientConfig config)
        {
            this.Config = config;
        }

        public async Task<CiContentItem> GetCiAsync(string ciIdentifier) =>
                (await GetCiResponseAsync(ciIdentifier).ConfigureAwait(false))?
                    .Content?
                    .FirstOrDefault();

        public async Task<AssignmentContentItem> GetAssignmentAsync(string name) =>
            (await GetAssignmentsResponseAsync(name).ConfigureAwait(false))?
                .Content?
                .FirstOrDefault();

        public Task UpdateDeploymentMethodAsync(string item, ConfigurationItemModel model) => PostAsync($"{Config.Endpoint}devices?{HttpUtility.UrlEncode(item)}", model);

        private Task<GetCiResponse> GetCiResponseAsync(string ciIdentifier) => GetAsync<GetCiResponse>($"{Config.Endpoint}devices?CiIdentifier={ciIdentifier}");

        private Task<GetAssignmentsResponse> GetAssignmentsResponseAsync(string name) => GetAsync<GetAssignmentsResponse>($"{Config.Endpoint}assignments?name={HttpUtility.UrlEncode(name)}");

        private async Task<T> GetAsync<T>(string url) =>
            await url.SetQueryParam("view", "expand")
                     .WithHeader("somecompany-apikey", Config.ApiKey)
                     .WithHeader("content-type", "application/json")
                     .GetJsonAsync<T>()
                     .ConfigureAwait(false);

        private async Task PostAsync(string url, object data) =>
            await url.WithHeader("somecompany-apikey", Config.ApiKey)
                     .WithHeader("content-type", "application/json")
                     .PostJsonAsync(data)
                     .ConfigureAwait(false);
    }
}
