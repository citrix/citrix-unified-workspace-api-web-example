/*
* Copyright © 2023. Cloud Software Group, Inc.
* This file is subject to the license terms contained
* in the license file that is distributed with this file.
*/

using Citrix.Unified.Api.Test.WebClient.CitrixOidc;
using Citrix.Unified.Api.Test.WebClient.Resources;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace Citrix.Unified.Api.Test.WebClient.Pages;

[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly ResourcesClient _resourcesClient;
    private readonly IDataProtectionProvider _dataProtectionProvider;
    private readonly string _applicationId;

    public IndexModel(ILogger<IndexModel> logger, ResourcesClient resourcesClient, IDataProtectionProvider dataProtectionProvider, IOptionsMonitor<OidcSettings> oidcSettings)
    {
        _logger = logger;
        _resourcesClient = resourcesClient;
        _dataProtectionProvider = dataProtectionProvider;
        _applicationId = oidcSettings.CurrentValue.ApplicationId;
    }

    public string? Domain { get; set; }

    public WspResourcesDto.WspResource[]? Resources { get; set; }

    public async Task<IActionResult> OnGet()
    {
        var sessionDetails = HttpContext.GetSessionDetails();
        if (sessionDetails == null)
        {
            return RedirectToPage("SignIn");
        }

        Domain = sessionDetails.WorkspaceDomain;


        try
        {
            Resources = await _resourcesClient.GetResources(sessionDetails);
        }
        catch (HttpRequestException err)
        {
            _logger.LogError(err, "Error retrieving resources");
            Resources = null;
        }

        return Page();
    }

    public string AddApplicationIdToNonCdnImageEndpoint(string url)
    {
        if (url.Contains("api/store/resources/images"))
        {
            return url + "?ApplicationId=" + _applicationId;
        }
        else
        {
            return url;
        }
    }

    public string Protect(string url)
    {
        return _dataProtectionProvider.CreateProtector("LaunchStatusUrl").Protect(url);
    }
}