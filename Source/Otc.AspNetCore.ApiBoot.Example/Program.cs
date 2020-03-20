﻿using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Otc.AspNetCore.ApiBoot.Example.WebApi
{
    [ExcludeFromCodeCoverage]
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
