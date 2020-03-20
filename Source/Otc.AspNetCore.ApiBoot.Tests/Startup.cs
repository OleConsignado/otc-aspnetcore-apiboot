using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Otc.AspNetCore.ApiBoot.Tests
{
    public class Startup : ApiBootStartup
    {
        public Startup(IConfiguration configuration)
            : base(configuration)
        {

        }

        protected override ApiMetadata ApiMetadata => new ApiMetadata
        {
            Name = "Otc.AspNetCore.ApiBoot.Tests",
            Description = "ApiBoot integration tests API",
            DefaultApiVersion = "1.0"
        };

        protected override void ConfigureApiServices(IServiceCollection services)
        {
            
        }
    }
}
