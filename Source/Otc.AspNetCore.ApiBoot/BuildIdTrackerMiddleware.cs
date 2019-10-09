using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Otc.AspNetCore.ApiBoot
{
    public class BuildIdTrackerMiddleware
    {
        public const string BuildIdHeaderKey = "X-Build-Id";

        private readonly RequestDelegate next;

        public BuildIdTrackerMiddleware(RequestDelegate next, string buildId)
        {
            this.next = next;
            this.buildId = buildId;
        }

        private readonly string buildId;

        public async Task Invoke(HttpContext context)
        {
            if (!string.IsNullOrEmpty(buildId))
            { 
                context.Response.OnStarting(
                    () =>
                    {
                        context.Response.Headers.Add(BuildIdHeaderKey, buildId);
                        return Task.CompletedTask;
                    });
            }

            await next(context);
        }
    }
}
