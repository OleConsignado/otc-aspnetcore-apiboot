using Microsoft.AspNetCore.Mvc;
using System;

namespace Otc.AspNetCore.ApiBoot
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [ApiVersionNeutral]
    public class HealthChecksController : ControllerBase
    {
        public const string RoutePath = "/healthz";

        [HttpGet(RoutePath)]
        public IActionResult Healthz()
        {
            return Ok(DateTimeOffset.Now);
        }
    }
}
