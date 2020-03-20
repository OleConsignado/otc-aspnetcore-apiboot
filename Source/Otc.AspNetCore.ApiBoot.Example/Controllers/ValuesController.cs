using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Otc.AspNetCore.ApiBoot;
using Otc.Caching.Abstractions;
using Otc.DomainBase.Exceptions;
using System;
using System.Collections.Generic;

namespace Otc.AspNetCore.ApiBoot.Example.WebApi.Controllers
{
    [ApiVersion("1.0")]
    [ApiVersion("1.1")]
    public class ValuesController : ApiController
    {
        private readonly Random rand = new Random();

        private string PickAValue()
        {
            return $"value{rand.Next(10)}";
        }

        // GET v1/values
        /// <summary>
        /// Get values.
        /// </summary>
        /// <response code="200">List with values.</response>
        /// <response code="500">Internal error.</response>
        [HttpGet, AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<string>), 200)]
        [ProducesResponseType(typeof(InternalError), 500)]
        [MapToApiVersion("1.0")]
        public IActionResult GetV1()
        {
            return Ok(new string[] { PickAValue(), PickAValue() });
        }


        // GET v1.1/values
        /// <summary>
        /// Get values.
        /// </summary>
        /// <response code="200">List with values.</response>
        /// <response code="500">Internal error.</response>
        [HttpGet, AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<string>), 200)]
        //[ProducesResponseType(typeof(InternalError), 500)]
        [MapToApiVersion("1.1")]
        public IActionResult GetV11([FromServices] ITypedCache typedCache)
        {
            if (typedCache == null)
            {
                throw new ArgumentNullException(nameof(typedCache));
            }

            var cacheKey = "values";

            // try get cached values
            if (!typedCache.TryGet(cacheKey, out string[] values))
            {
                // if cache not found or it has expired, then create new values
                values = new string[] { PickAValue(), PickAValue() };

                // store values in cache for 60 seconds
                typedCache.Set(cacheKey, values, TimeSpan.FromSeconds(60));
            }

            return Ok(values);
        }

        // GET v1/values/5
        // GET v1.1/values/5
        /// <summary>
        /// Get specific value.
        /// </summary>
        /// <response code="200">Value for the given id.</response>
        /// <response code="404">Value not found for the given id.</response>
        /// <response code="500">Internal error.</response>
        [HttpGet("{id}"), AllowAnonymous]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(404)]
        //[ProducesResponseType(typeof(InternalError), 500)]
        public IActionResult Get(int id)
        {
            if (id == 5)
            {
                return Ok("value");
            }
            else
            {
                return NotFound();
            }
        }

        // POST v1/values
        // POST v1.1/values
        /// <summary>
        /// Post (create) new value (require authentication).
        /// </summary>
        /// <response code="204">Item sucessfuly created.</response>
        /// <response code="401">Not authorized, you must provide a valid Authorization Bearer Token (header) in order to get access to this operation.</response>
        /// <response code="500">Internal error.</response>
        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        //[ProducesResponseType(typeof(InternalError), 500)]
        public IActionResult Post([FromBody] string value)
        {
            // create item with the given value

            return NoContent();
        }

        // PUT v1/values/5
        // PUT v1.1/values/5
        /// <summary>
        /// Put (update) value with the given id (require authentication).
        /// </summary>
        /// <response code="204">Item sucessfuly updated.</response>
        /// <response code="401">Not authorized, you must provide a valid Authorization Bearer Token (header) in order to get access to this operation.</response>
        /// <response code="404">Value not found for the given id.</response>
        /// <response code="500">Internal error.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        //[ProducesResponseType(typeof(InternalError), 500)]
        public IActionResult Put(int id, [FromBody] string value)
        {
            if (id == 5)
            {
                // update item with id 5

                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }

        // DELETE v1/values/5
        // DELETE v1.1/values/5
        /// <summary>
        /// Delete value with the given id (require authentication).
        /// </summary>
        /// <response code="204">Item sucessfuly deleted.</response>
        /// <response code="401">Not authorized, you must provide a valid Authorization Bearer Token (header) in order to get access to this operation.</response>
        /// <response code="404">Value not found for the given id.</response>
        /// <response code="500">Internal error.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        //[ProducesResponseType(typeof(InternalError), 500)]
        public IActionResult Delete(int id)
        {
            if (id == 5)
            {
                // delete item with id 5

                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }
    }
}
