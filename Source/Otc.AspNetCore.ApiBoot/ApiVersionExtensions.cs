using Microsoft.AspNetCore.Builder;

namespace Otc.AspNetCore.ApiBoot
{
    public static class ApiVersionExtensions
    {
        public static IApplicationBuilder UseApiBootVersion(this IApplicationBuilder app,
            string version)
        {
            return app.UseMiddleware<ApiVersionMiddleware>(version);
        }
    }
}
