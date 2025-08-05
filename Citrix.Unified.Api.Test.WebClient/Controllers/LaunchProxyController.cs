// Copyright © 2025. Cloud Software Group, Inc. All Rights Reserved.

using Citrix.Unified.Api.Test.WebClient.Resources;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;

namespace Citrix.Unified.Api.Test.WebClient.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LaunchProxyController : ControllerBase
    {

        private readonly ResourcesClient _resourcesClient;
        private readonly IDataProtectionProvider _dataProtectionProvider;

        public LaunchProxyController(ResourcesClient resourcesClient, IDataProtectionProvider dataProtectionProvider)
        {
            _resourcesClient = resourcesClient;
            _dataProtectionProvider = dataProtectionProvider;
        }

        [HttpGet("icafile", Name ="GetIceFileProxy")]
        public async Task<string> GetIcaFileProxy([FromQuery] string launchUrl)
        {
            var targetUrl = _dataProtectionProvider.CreateProtector("LaunchStatusUrl").Unprotect(launchUrl);
            return await _resourcesClient.GetIcaFile(targetUrl);

        }
    }
}
