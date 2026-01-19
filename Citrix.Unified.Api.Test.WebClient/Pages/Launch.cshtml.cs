/*
* Copyright © 2025. Cloud Software Group, Inc.
* This file is subject to the license terms contained
* in the license file that is distributed with this file.
*/

using System.ComponentModel.DataAnnotations;

using Citrix.Unified.Api.Test.WebClient.Resources;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Citrix.Unified.Api.Test.WebClient.Pages
{
    public class LaunchModel : PageModel
    {
        private readonly ILogger<LaunchModel> _logger;
        private readonly ResourcesClient _resourcesClient;
        private readonly IDataProtectionProvider _dataProtectionProvider;

        public LaunchModel(ILogger<LaunchModel> logger, ResourcesClient resourcesClient, IDataProtectionProvider dataProtectionProvider)
        {
            _logger = logger;
            _resourcesClient = resourcesClient;
            _dataProtectionProvider = dataProtectionProvider;
        }

        public bool? Retry { get; set; }

        public string? ErrorMessage { get; set; }

        [BindProperty]
        [Required]
        public string Id { get; set; }


        [BindProperty]
        [Required]
        public string ProtectedFileFetchUrl { get; set; }

        public IActionResult OnGet()
        {
            ErrorMessage = "No data supplied";
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                _logger.LogInformation("Failed model state", ModelState.ToString());
                return BadRequest();
            }

            var fileFetchUrl = UnProtect(ProtectedFileFetchUrl);

            WspResourceLaunchDto launchData;
            try
            {
                launchData = await _resourcesClient.AttemptLaunch(fileFetchUrl);
            }
            catch
            {
                ErrorMessage = "Error launching resource";
                return Page();
            }

            switch (launchData.Status)
            {
                case "success":
                    {
                        _logger.LogInformation("Sending to CWA url, {ReceiverUri}", launchData.ReceiverUri);
                        return Redirect(launchData.ReceiverUri);
                    }
                case "retry":
                    Retry = true;
                    return Page();
                default:
                    ErrorMessage = "Unexpected response - " + launchData.Status;
                    return Page();
            }
        }

        public string UnProtect(string protectedUrl)
        {
            return _dataProtectionProvider.CreateProtector("FileFetchUrl").Unprotect(protectedUrl);
        }
    }
}