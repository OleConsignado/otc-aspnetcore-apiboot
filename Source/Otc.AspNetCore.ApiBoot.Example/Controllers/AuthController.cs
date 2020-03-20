using Otc.AspNetCore.ApiBoot.Example.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Otc.AspNetCore.ApiBoot;
using Otc.AuthorizationContext.Abstractions;
using Otc.DomainBase.Exceptions;

namespace Otc.AspNetCore.ApiBoot.Example.Controllers
{
    public class AuthController : ApiController
    {
        // POST v1/authentication
        /// <summary>
        /// Authenticate (provide a non empty creedential argument).
        /// </summary>
        /// <param name="creedentials">A non empty string will handled as a valid creedential (for example purpose only)</param>
        /// <response code="200">String with Jwt Token.</response>
        /// <response code="401">Invalid creedentials.</response>
        /// <response code="500">Internal error.</response>
        [HttpPost, AllowAnonymous]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(401)]
        //[ProducesResponseType(typeof(InternalError), 500)]
        public IActionResult Post(
            [FromServices] IAuthorizationDataSerializer<AuthorizationData> authorizationDataSerializer,
            [FromBody] string creedentials)
        {
            if(!string.IsNullOrEmpty(creedentials)) // confirm creedentials here
            {
                var token = authorizationDataSerializer.Serialize(new AuthorizationData()
                {
                    UserId = "A123",
                    MyCustomAuthData = "Some custom authorization data here!"
                });

                return Ok(token);
            }

            return Unauthorized();
        }
    }
}
