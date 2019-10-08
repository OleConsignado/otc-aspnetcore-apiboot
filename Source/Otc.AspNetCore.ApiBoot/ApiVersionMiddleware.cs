using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Otc.AspNetCore.ApiBoot
{
    public class ApiVersionMiddleware
    {
        private readonly RequestDelegate next;

        public ApiVersionMiddleware(RequestDelegate next, string version)
        {
            this.next = next;
            this.Version = version;
        }

        private string Version { get; }

        public async Task Invoke(HttpContext context)
        {
            if (!string.IsNullOrEmpty(Version))
            { 
                context.Response.OnStarting(
                    () =>
                    {
                        context.Response.Headers.Add("X-Api-Version", Version);
                        return Task.CompletedTask;
                    });
            }

            await next(context);
        }
    }
}
