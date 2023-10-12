﻿/*
* Copyright © 2023. Cloud Software Group, Inc.
* This file is subject to the license terms contained
* in the license file that is distributed with this file.
*/

using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Citrix.Unified.Api.Test.WebClient.CitrixOidc;

/// <summary>
/// This handles adding additional values to the outgoing requests.
/// </summary>
public class CitrixOpenIdConnectEvents : OpenIdConnectEvents
{
    public override Task RedirectToIdentityProvider(RedirectContext context)
    {
        context.ProtocolMessage.AcrValues = context.Properties.GetParameter<string>(OpenIdConnectParameterNames.AcrValues);
        return Task.CompletedTask;
    }

    public override Task AuthorizationCodeReceived(AuthorizationCodeReceivedContext context)
    {
        if (context.TokenEndpointRequest == null)
        {
            return Task.CompletedTask;
        }

        return Task.CompletedTask;
    }
}