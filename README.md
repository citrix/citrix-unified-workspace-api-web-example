# Citrix(R) Unified Workspace API - Web Client sample built with .Net Core

This sample is a small web application, built on .NET Core, that shows how to interact with the Citrix(R) Unified Workspace APIs. This is intended to demonstrate the APIs if you would like to use some of this functionality into your own applications/web sites.

The application is a web-based client that allows the user to login via the Citrix Authorization server (and your configured Identity Provider), obtain an OAuth Access and Refresh Token, and call the Workspace APIs.

This is purely an example and shouldn't be used for real production services. The entire end-to-end flow is contained within a single application including storing the Client details.

## Sample site preview

![Homepage](./screenshots/Resources.PNG)

## Prerequisites

- You have either a Private or Public Workspace OAuth Client
- You will be running the example code in Visual Studio and can run [.NET 7.0](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)
- You have the [Citrix Workspace App](https://www.citrix.com/downloads/workspace-app/windows/workspace-app-for-windows-latest.html) installed

## Getting Started

### Https Self-Signed Certificate

You need to have an have a local self-signed developer certificate created and trusted. If you are using Visual Studio this should be taken care for you. See the `dotnet dev-certs` command, https://learn.microsoft.com/en-us/dotnet/core/additional-tools/self-signed-certificates-guide.

### Configuration

For this to work out of the box, your Workspace OAuth Client will need to have `https://localhost:7182/callback` added as an allowed redirect URL.

#### Workspace OAuth Client Settings

To configure the Workspace OAuth Client settings including secrets use the dotnet user secrets feature. See https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-6.0&tabs=windows.

Example:

```json
{
  "Client": {
    "ClientId": "clientId==",
    "ClientSecret": null,
    "CallbackPath": "/callback",
    "UsePkce": true,
    "UseOfflineAccess": true
  }
}
```

- [Required] `ClientId`: The Public or Private client ID returned during client creation.
- [Optional] `ClientSecret`: Only provide this for Private Clients, should have been returned during creation.
- [Optional] `CallbackPath`: Set to `/callback` by default, this is used to formulate the 'redirect url' that is required to be set on the client, e.g. the host for this application is `https://localhost:7182` and therefore the allowed redirect URL set on the client must be `https://localhost:7182/callback`.
- [Optional] `UsePkce`: Set to `true` by default, this must match what you set during client creation.
- [Optional] `UseOfflineAccess`: Set to `true` by default, this must match what you set during client creation.

## Running the example

- Recommend using an Incognito browser to avoid cookie and cache causing problems.

Once started, you should be directed to the login page:

![Login Screen](./screenshots/Login.PNG)

At this point, enter your customer.cloud.com address and hit 'login'.

It should direct you to the standard login flow and after entering your login details it will take you to the home page:

![Homepage](./screenshots/Resources.PNG)

Clicking on the resources should initiate a launch:

![Start Launch](./screenshots/Launch_Initial.PNG)

![CWA Launch](./screenshots/CWA_Opening_App.PNG)

![Launch success](./screenshots/Calculator_Launched.PNG)

You can view the Token information by going to the `/RefreshToken` endpoint:

![Refresh](./screenshots/RefreshToken.PNG)

## What could go wrong

If you are presented with this page:
![Error](./screenshots/Error.PNG)

it's likely that you've mis-configured your client. Please confirm the redirect URLs setup for the client and ensure it includes `https://localhost:7182/callback`, or something else if you've modified the launch settings.

If that doesn't seem to be the issue, please contact Support.

## Javascript Libraries

The repo includes the following javascript libraries,

- Bootstrap, https://getbootstrap.com/
- jQuery,  https://jquery.com/

## License

Copyright © 2023. Cloud Software Group, Inc. All Rights Reserved.
