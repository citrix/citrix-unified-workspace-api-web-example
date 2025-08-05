/*
* Copyright © 2025. Cloud Software Group, Inc.
* This file is subject to the license terms contained
* in the license file that is distributed with this file.
*/

using Citrix.Unified.Api.Test.WebClient.Discovery;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Citrix.Unified.Api.Test.WebClient.CitrixOidc
{
    public static class CitrixAuthExtensions
    {
        public const string ItemWorkspaceDomain = "workspace_domain";

        public static ChallengeResult CreateChallenge(this DiscoveryModel model, Boolean useOfflineAccess)
        {
            var scopes = new[] { "openid", "wsp", "spa", "leases" }.ToList();
            if(useOfflineAccess)
            {
                scopes.Add("offline_access");
            }

            return new ChallengeResult(OpenIdConnectDefaults.AuthenticationScheme, new OpenIdConnectChallengeProperties()
            {
                Parameters =
                {
                    {OpenIdConnectParameterNames.AcrValues, model.ClientSettings.AcrValues}
                },
                Items =
                {
                    {ItemWorkspaceDomain, model.ClientSettings.CustomerDomain}
                },
                Scope = scopes,
                Prompt = model.ClientSettings.PromptLoginEnabled ? "Login" : null,
                RedirectUri = "/"
            });
        }

        public static bool IsLoggedIn(this HttpContext context)
        {
            var authenticateResultFeature = context.Features.Get<IAuthenticateResultFeature>();
            return authenticateResultFeature?.AuthenticateResult?.Succeeded is true;
        }

        public static SessionDetails? GetSessionDetails(this HttpContext context)
        {
            var authenticateResultFeature = context.Features.Get<IAuthenticateResultFeature>()?.AuthenticateResult;

            if (authenticateResultFeature?.Succeeded is not true)
            {
                return null;
            }

            var domainClaim = authenticateResultFeature.Properties.GetString(ItemWorkspaceDomain);
            var tokenAsync = authenticateResultFeature.Properties.GetTokenValue("access_token");

            if (tokenAsync == null || domainClaim == null)
            {
                return null;
            }
            return new SessionDetails(domainClaim, tokenAsync);
        }
    }
}