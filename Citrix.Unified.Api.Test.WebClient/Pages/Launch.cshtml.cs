/*
* Copyright © 2023. Cloud Software Group, Inc.
* This file is subject to the license terms contained
* in the license file that is distributed with this file.
*/

using System.ComponentModel.DataAnnotations;
using System.Text;

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
        public string ProtectedLaunchStatusUrl { get; set; }

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

            var launchStatusUrl = UnProtect(ProtectedLaunchStatusUrl);

            WspResourceLaunchDto launchData;
            try
            {
                launchData = await _resourcesClient.AttemptLaunch(launchStatusUrl);
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
                        var receiverUrl = BuildReceiverUrl(launchData);

                        _logger.LogInformation("Sending to CWA url, {receiverUrl}", receiverUrl);
                        return Redirect(receiverUrl);
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
            return _dataProtectionProvider.CreateProtector("LaunchStatusUrl").Unprotect(protectedUrl);
        }

        private static string BuildReceiverUrl(WspResourceLaunchDto launchData)
        {
            var queryString = QueryString.Create(new Dictionary<string, string?>()
            {
                {"action", "launch"},
                {"transport", "https"},
                {"serverProtocolVersion", launchData.ServerProtocolVersion},
                {"ticket", launchData.FileFetchTicket}
            });

            if (launchData.FileFetchStaTicket != null)
            {
                queryString = queryString.Add("staTicket", launchData.FileFetchStaTicket);
            }

            var base64EncodedParameters = Convert.ToBase64String(Encoding.UTF8.GetBytes(queryString.ToUriComponent()))
                .Replace("+", "_")
                .Replace("/", "!")
                .Replace("=", "-");

            var fetchUrl = launchData.FileFetchUrl ?? throw new InvalidOperationException("FileFetch url should not be null");
            var uriBuilder = new UriBuilder(fetchUrl);

            uriBuilder.Scheme = "receiver";
            uriBuilder.Path = uriBuilder.Path + "/" +
                              base64EncodedParameters;

            return uriBuilder.Uri.ToString();
        }
    }
}