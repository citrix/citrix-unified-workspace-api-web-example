/*
* Copyright © 2025. Cloud Software Group, Inc.
* This file is subject to the license terms contained
* in the license file that is distributed with this file.
*/

namespace Citrix.Unified.Api.Test.WebClient
{
    /// <summary>
    /// Handler to automatically add the `Citrix-TransactionId` header and log response information.
    /// </summary>
    internal class CitrixHttpMessageHandler : DelegatingHandler
    {
        private readonly ILogger<CitrixHttpMessageHandler> _logger;

        public CitrixHttpMessageHandler(ILogger<CitrixHttpMessageHandler> logger)
        {
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string? guid;
            if (!request.Headers.TryGetValues("Citrix-TransactionId", out var transactionIdHeaders) || (guid = transactionIdHeaders.FirstOrDefault()) == null)
            {
                guid = Guid.NewGuid().ToString();
                request.Headers.Add("Citrix-TransactionId", guid);
            }
            _logger.LogInformation("Sending request to {method}:{url}, {guid}", request.Method, request.RequestUri, guid);

            var response = await base.SendAsync(request, cancellationToken);

            var errorSource = response.Headers.TryGetValues("Citrix-Error-Source", out var headers) ? string.Join(",", headers) : "";

            _logger.LogInformation("Received Response: {status} {reasonPhrase} {errorSource}, {guid}", response.StatusCode, response.ReasonPhrase, errorSource, guid);
            return response;
        }
    }
}