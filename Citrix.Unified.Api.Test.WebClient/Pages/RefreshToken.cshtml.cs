/*
* Copyright © 2025. Cloud Software Group, Inc.
* This file is subject to the license terms contained
* in the license file that is distributed with this file.
*/

using Duende.AccessTokenManagement.OpenIdConnect;

using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Citrix.Unified.Api.Test.WebClient.Pages
{
    public class RefreshTokenModel : PageModel
    {
        public string MaskedAccessToken { get; set; }
        public string MaskedRefreshToken { get; set; }

        public string Expires { get; set; }
        public string Scopes { get; set; }

        public async Task OnGet()
        {
            var tokenResult = await HttpContext.GetUserAccessTokenAsync();

            if (tokenResult.Token != null)
            {
                MaskedAccessToken = Mask(tokenResult.Token.AccessToken);
                MaskedRefreshToken = Mask(tokenResult.Token.RefreshToken);
                Expires = tokenResult.Token.Expiration.ToString() + " -- (" + (tokenResult.Token.Expiration - DateTimeOffset.UtcNow).TotalHours + " hours from now)";
            }
            else
            {
                MaskedAccessToken = "Error retrieving token";
            }
        }

        public async Task OnPost()
        {
            // Force token renewal
            var tokenResult = await HttpContext.GetUserAccessTokenAsync(new UserTokenRequestParameters());

            if (tokenResult.Token != null)
            {
                MaskedAccessToken = Mask(tokenResult.Token.AccessToken);
                MaskedRefreshToken = Mask(tokenResult.Token.RefreshToken);
                Expires = tokenResult.Token.Expiration.ToString() + " -- (" + (tokenResult.Token.Expiration - DateTimeOffset.UtcNow).TotalHours + " hours from now)";
            }
            else
            {
                MaskedAccessToken = "Error refreshing token";
            }
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