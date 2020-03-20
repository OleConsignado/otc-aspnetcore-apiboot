using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Otc.AspNetCore.ApiBoot.Tests.Controllers
{
    [ApiVersion("1.0")]
    public class TestsController : ApiController
    {
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                Test = "Ok"
            });
        }

        [AllowAnonymous]
        [HttpGet("Exception")]
        public IActionResult ProduceException()
        {
            throw new Exception("Error");
        }
    }
}
