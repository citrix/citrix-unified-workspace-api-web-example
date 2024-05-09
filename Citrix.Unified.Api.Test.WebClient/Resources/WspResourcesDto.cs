/*
* Copyright © 2023. Cloud Software Group, Inc.
* This file is subject to the license terms contained
* in the license file that is distributed with this file.
*/

using static Citrix.Unified.Api.Test.WebClient.Resources.WspResourcesDto;

namespace Citrix.Unified.Api.Test.WebClient.Resources
{
    public record WspResourcesDto(WspResource[] Resources)
    {
        public record WspResource(string? ResourceId, string? Name, bool? Enabled, Links Links);

        public record Links(string ImageUrl, string LaunchStatusUrl, string LaunchUrl);
    }

    public record WspResourceLaunchDto(
        string? Status,
        int? PollTimeout,
        string? FileFetchUrl,
        string? ServerProtocolVersion,
        string? FileFetchTicket,
        string? FileFetchStaTicket,
        string? ReceiverUri,
        string? ErrorId,
        bool? SuggestRestart,
        string? RetryUrl
        );
}