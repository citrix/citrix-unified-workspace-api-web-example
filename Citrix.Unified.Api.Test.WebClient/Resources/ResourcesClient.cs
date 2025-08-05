/*
* Copyright © 2025. Cloud Software Group, Inc.
* This file is subject to the license terms contained
* in the license file that is distributed with this file.
*/

using Citrix.Unified.Api.Test.WebClient.CitrixOidc;
using Citrix.Unified.Api.Test.WebClient.Discovery;

namespace Citrix.Unified.Api.Test.WebClient.Resources
{
    public class ResourcesClient
    {
        private readonly ILogger<ResourcesClient> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly DiscoveryClient _discoveryClient;

        public ResourcesClient(ILogger<ResourcesClient> logger, IHttpClientFactory httpClientFactory, DiscoveryClient discoveryClient)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _discoveryClient = discoveryClient;
        }

        public async Task<WspResourcesDto.WspResource[]> GetResources(SessionDetails sessionDetails, bool cached = false)
        {
            var serviceEndpointUrl = await _discoveryClient.GetServiceEndpointUrl(sessionDetails.WorkspaceDomain, "store", "ListResources");

            var httpClient = _httpClientFactory.CreateClient("apiClient");

            var httpResponseMessage = await httpClient.GetAsync(serviceEndpointUrl);

            httpResponseMessage.EnsureSuccessStatusCode();

            var readFromJsonAsync = await httpResponseMessage.Content.ReadFromJsonAsync<WspResourcesDto>();

            return readFromJsonAsync!.Resources;
        }

        public async Task<WspResourceLaunchDto> AttemptLaunch(string launchStatusUrl)
        {
            var httpClient = _httpClientFactory.CreateClient("apiClient");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, launchStatusUrl);
            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

            httpResponseMessage.EnsureSuccessStatusCode();

            return await httpResponseMessage.Content.ReadFromJsonAsync<WspResourceLaunchDto>()
                   ?? throw new InvalidOperationException();
        }
        
        public async Task<string> GetIcaFile(string launchUrl)
        {
            var httpClient = _httpClientFactory.CreateClient("apiClient");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, launchUrl);
            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

            httpResponseMessage.EnsureSuccessStatusCode();

            return await httpResponseMessage.Content.ReadAsStringAsync()
                   ?? throw new InvalidOperationException();
        }
    }
}