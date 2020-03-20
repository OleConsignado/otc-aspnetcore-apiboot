using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Otc.AspNetCore.ApiBoot;
using System.Diagnostics.CodeAnalysis;

namespace Otc.AspNetCore.ApiBoot.Example.WebApi
{
    /// <summary>
    /// API Startup class. 
    /// </summary>
    public class Startup : ApiBootStartup
    {
        protected override ApiMetadata ApiMetadata => new ApiMetadata()
        {
            Name = "Otc.AspNetCore.ApiBoot.Example",
            Description = "ApiBoot based API (https://github.com/OleConsignado/otc-aspnetcore-apiboot)",
            DefaultApiVersion = "1.0"
        };

        public Startup(IConfiguration configuration)
            : base(configuration)
        {
            // Disable request body capturing for AuthController in order
            // to prevent capture creedentials
            RequestTrackingDisableBodyCapturingForUrl("v[0-9.]+/auth");
        }

        /// <summary>
        /// API service registration.
        /// </summary>
        /// <param name="services">The service collection</param>
        [ExcludeFromCodeCoverage]
        protected override void ConfigureApiServices(IServiceCollection services)
        {
            // API service registration goes here
        }
    }
}
