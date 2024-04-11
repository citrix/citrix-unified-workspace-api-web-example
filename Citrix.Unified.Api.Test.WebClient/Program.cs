﻿/*
* Copyright © 2023. Cloud Software Group, Inc.
* This file is subject to the license terms contained
* in the license file that is distributed with this file.
*/

using Citrix.Unified.Api.Test.WebClient;
using Citrix.Unified.Api.Test.WebClient.CitrixOidc;
using Citrix.Unified.Api.Test.WebClient.Discovery;
using Citrix.Unified.Api.Test.WebClient.Resources;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

var builder = WebApplication.CreateBuilder(args);

var oidcSettings = builder.Configuration.GetSection("Client").Get<OidcSettings>();

builder.Logging.AddConsole();

// Add services to the container.
var services = builder.Services;

services.Configure<OidcSettings>(builder.Configuration.GetSection("Client"));

services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    .AddCookie(
        options =>
        {
            options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
            options.SlidingExpiration = true;

            // automatically revoke refresh token at signout time
            options.Events.OnSigningOut = async e =>
            {
                await e.HttpContext.RevokeRefreshTokenAsync();
            };

        }
        )
    .AddOpenIdConnect(options =>
    {
        options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.Authority = oidcSettings.Authority;
        options.ResponseType = OpenIdConnectResponseType.Code;

        options.ClientId = oidcSettings.ClientId;
        options.ClientSecret = oidcSettings.ClientSecret;

        options.UsePkce = oidcSettings.UsePkce;

        options.CallbackPath = oidcSettings.CallbackPath;
        options.SaveTokens = true;
        options.Events = new CitrixOpenIdConnectEvents();
        options.UseTokenLifetime = true;
    });

builder.Services.AddOpenIdConnectAccessTokenManagement(options =>
{

});

services.AddRazorPages();

services.AddDataProtection();

services.AddHealthChecks();

services.AddHttpClient(nameof(DiscoveryClient));
services.AddMemoryCache();
services.AddSingleton<DiscoveryClient>();

services.AddHttpClient("apiClient", configureClient: client =>
{
    client.DefaultRequestHeaders.UserAgent.ParseAdd("Citrix WebApp Example - API HttpClient");
    client.DefaultRequestHeaders.Add("Citrix-ApplicationId", oidcSettings.ApplicationId);
})
    .AddHttpMessageHandler<CitrixHttpMessageHandler>()
    .AddUserAccessTokenHandler();

services.AddSingleton<ResourcesClient>();

services.AddTransient<CitrixHttpMessageHandler>();
services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.MapRazorPages();

app.MapControllers();

app.Run();