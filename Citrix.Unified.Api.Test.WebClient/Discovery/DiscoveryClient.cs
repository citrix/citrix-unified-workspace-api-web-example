/*
* Copyright © 2023. Cloud Software Group, Inc.
* This file is subject to the license terms contained
* in the license file that is distributed with this file.
*/

using System.Net;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Citrix.Unified.Api.Test.WebClient.Discovery;

public class DiscoveryClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMemoryCache _memoryCache;


    public DiscoveryClient(IHttpClientFactory httpClientFactory, IMemoryCache memoryCache)
    {
        _httpClientFactory = httpClientFactory;
        _memoryCache = memoryCache;
    }

    public async Task<DiscoveryModel?> GetCustomerData(string customerDomain)
    {
        var escapedCustomerDomain = Uri.EscapeDataString(customerDomain);
        return await _memoryCache.GetOrCreateAsync("DiscoveryDocuments_" + escapedCustomerDomain, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);

            var uri = new UriBuilder(customerDomain)
            {
                Path = $"/api/discovery/configurations"
            }.Uri;

            var httpClient = _httpClientFactory.CreateClient(nameof(DiscoveryClient));

            var httpResponseMessage = await httpClient.GetAsync(uri);

            if (httpResponseMessage.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            httpResponseMessage.EnsureSuccessStatusCode();

            return await httpResponseMessage.Content.ReadFromJsonAsync<DiscoveryModel>();

        });
    }

    public async Task<Uri?> GetServiceEndpointUrl(string customerDomain, string service, string endpoint)
    {
        var discoveryModel = await GetCustomerData(customerDomain);
        if (discoveryModel == null)
        {
            return null;
        }

        var url = discoveryModel.Services
            .FirstOrDefault(serviceInfo => service.Equals(serviceInfo.Service))?.Endpoints
            .FirstOrDefault(endpoint1 => endpoint1.Id.Equals(endpoint))?.Url;

        return Uri.TryCreate(url, UriKind.Absolute, out var result) ? result : null;
    }
}