using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Otc.Caching.DistributedCache.All;
using Otc.AuthorizationContext.AspNetCore.Jwt;
using System;
using System.Collections.Generic;

namespace Otc.AspNetCore.ApiBoot.TestHost
{
    public class HttpFixture<TStartup>
        where TStartup : ApiBootStartup
    {
        public TestServer CreateServer(Action<IServiceCollection> serviceConfiguration)
        {
            var builder = WebHost.CreateDefaultBuilder()
                .UseSetting(nameof(JwtConfiguration.Audience), StaticConfiguration.JwtConfiguration.Audience)
                .UseSetting(nameof(JwtConfiguration.Issuer), StaticConfiguration.JwtConfiguration.Issuer)
                .UseSetting(nameof(JwtConfiguration.SecretKey), StaticConfiguration.JwtConfiguration.SecretKey)
                .UseSetting(nameof(ApiBootOptions.EnableSwagger), "False")
                .UseSetting(nameof(ApiBootOptions.EnableLogging), "False")
                .UseSetting(nameof(DistributedCacheConfiguration.CacheStorageType), StorageType.Memory.ToString())
                .ConfigureServices(serviceConfiguration)
                .UseStartup<TStartup>();

            return new TestServer(builder);
        }
    }
}
