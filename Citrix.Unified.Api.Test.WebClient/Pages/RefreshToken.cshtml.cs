/*
* Copyright © 2025. Cloud Software Group, Inc.
* This file is subject to the license terms contained
* in the license file that is distributed with this file.
*/

using Duende.AccessTokenManagement.OpenIdConnect;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Citrix.Unified.Api.Test.WebClient.Pages
{
    public class RefreshTokenModel : PageModel
    {
        private readonly IUserTokenManagementService _userTokenManagementService;

        public RefreshTokenModel(IUserTokenManagementService userTokenManagementService)
        {
            _userTokenManagementService = userTokenManagementService;
        }

        public string MaskedAccessToken { get; set; }
        public string MaskedRefreshToken { get; set; }

        public string Expires { get; set; }
        public string Scopes { get; set; }

        public async Task OnGet()
        {
            var accessTokenAsync = await _userTokenManagementService.GetAccessTokenAsync(HttpContext.User);

            Populate(accessTokenAsync);
        }

        public async Task OnPost()
        {
            await HttpContext.GetUserAccessTokenAsync(new UserTokenRequestParameters() { ForceRenewal = true });

            var accessTokenAsync = await _userTokenManagementService.GetAccessTokenAsync(HttpContext.User);

            Populate(accessTokenAsync);
        }

        public void Populate(UserToken userToken)
        {

            MaskedAccessToken = Mask(userToken.AccessToken);
            MaskedRefreshToken = Mask(userToken.RefreshToken);
            Expires = userToken.Expiration.ToString() + " -- (" + (userToken.Expiration - DateTimeOffset.UtcNow).TotalHours + " hours from now)";

        }

        private string Mask(string? input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return "";
            }

            var n = input.Length;
            var toShow = (int)Math.Floor(Math.Sqrt(n));
            return $"'{input[..toShow]}*************' (hash = {(uint)input.GetHashCode()})";
        }
    }
}