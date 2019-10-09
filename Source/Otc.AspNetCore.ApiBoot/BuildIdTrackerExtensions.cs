using Microsoft.AspNetCore.Builder;

namespace Otc.AspNetCore.ApiBoot
{
    public static class BuildIdTrackerExtensions
    {
        public static IApplicationBuilder UseBuildIdTracker(this IApplicationBuilder app,
            string version)
        {
            return app.UseMiddleware<BuildIdTrackerMiddleware>(version);
        }
    }
}
