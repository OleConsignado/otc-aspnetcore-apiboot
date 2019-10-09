using Microsoft.AspNetCore.Builder;

namespace Otc.AspNetCore.ApiBoot
{
    public static class BuildVersionExtensions
    {
        public static IApplicationBuilder UseBuildVersion(this IApplicationBuilder app,
            string version)
        {
            return app.UseMiddleware<BuildVersionMiddleware>(version);
        }
    }
}
