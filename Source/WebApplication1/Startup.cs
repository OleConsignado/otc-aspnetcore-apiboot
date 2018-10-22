using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Otc.AspNetCore.ApiBoot;

namespace WebApplication1
{
    public class Startup : ApiBootStartup
    {
        protected override ApiMetadata ApiMetadata => new ApiMetadata()
        {
            Name = "Olé Open API - Propostas",
            Description = "API de acesso a recursos e operações relacionados a propostas",
            DefaultApiVersion = "1.0"
        };

        public Startup(IConfiguration configuration)
            : base(configuration)
        {
        }

        protected override void ConfigureApiServices(IServiceCollection services)
        {            
        }
    }
}
