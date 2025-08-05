/*
* Copyright © 2025. Cloud Software Group, Inc.
* This file is subject to the license terms contained
* in the license file that is distributed with this file.
*/

using System.ComponentModel.DataAnnotations;

using Citrix.Unified.Api.Test.WebClient.CitrixOidc;
using Citrix.Unified.Api.Test.WebClient.Discovery;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace Citrix.Unified.Api.Test.WebClient.Pages
{
    public class SignInModel : PageModel
    {
        private readonly DiscoveryClient _client;
        private readonly IOptions<OidcSettings> _oidcSettings;

        public SignInModel(DiscoveryClient client, IOptions<OidcSettings> oidcSettings)
        {
            _client = client;
            _oidcSettings = oidcSettings;
        }

        [BindProperty]
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9][a-zA-Z0-9-]+\.[a-zA-Z0-9][a-zA-Z0-9-]+\.[a-zA-Z]{2,63}$")]
        public string? CustomerDomain { get; set; }

        public IActionResult OnGet()
        {
            if (HttpContext.IsLoggedIn())
            {
                return RedirectToPage("Index");
            }
            return Page();
        }

        public string? ErrorMessage;

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid || CustomerDomain == null)
            {
                return ErrorPage();
            }

            var discoveryModel = await _client.GetCustomerData(CustomerDomain);

            if (discoveryModel == null)
            {
                return ErrorPage();
            }

            return discoveryModel.CreateChallenge(_oidcSettings.Value.UseOfflineAccess);
        }

        private IActionResult ErrorPage()
        {
            ErrorMessage = "Invalid or Unknown Customer Domain";
            return Page();
        }
    }
}