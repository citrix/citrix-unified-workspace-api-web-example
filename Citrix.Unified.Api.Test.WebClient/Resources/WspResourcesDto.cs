/*
* Copyright © 2025. Cloud Software Group, Inc.
* This file is subject to the license terms contained
* in the license file that is distributed with this file.
*/

using static Citrix.Unified.Api.Test.WebClient.Resources.WspResourcesDto;

namespace Citrix.Unified.Api.Test.WebClient.Resources
{
    public record WspResourcesDto(WspResource[] Resources)
    {
        public record WspResource(string? ResourceId, string? Name, bool? Enabled, Links Links);

        public record Links(string ImageUrl, string IcaFileFetchTicketUrl, string IcaFileUrl);
    }

    public record WspResourceLaunchDto(
        string? Status,
        int? PollTimeout,
        string? ReceiverUri,
        string? ErrorId,
        bool? SuggestRestart,
        string? RetryUrl
        );
}