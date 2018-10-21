# Welcome to Otc.AspNetCore.ApiBoot
[![Build Status](https://travis-ci.org/OleConsignado/otc-aspnetcore-apiboot.svg?branch=master)](https://travis-ci.org/OleConsignado/otc-aspnetcore-apiboot)

The ApiBoot goal is to reduce boilerplate while creating new *AspNet Core WebApi* projects. It brings a lot of necessary stuff that almost all API should provide. 

So you can keep focus on the real problem you need to address instead of spend time configuring logs, authentication/authorization, swagger and more.

# ApiBootStartup

The key element of **ApiBoot** is the `ApiBootStartup` class which was designed to be the base class for the regular *AspNet Core WebApi* `Startup` class.

`ApiBootStartup` already implements the regular `Configure` and `ConfigureServices` startup methods and it require you to implement the`ConfigureApiServices` abstract method, where you place service registration logic for your custom services and `ApiMetadata` abstract get property, where you provide some metadata.

**Startup example:**


```cs 
public class Startup : ApiBootStartup
{
    protected override ApiMetadata ApiMetadata => new ApiMetadata()
    {
        Name = "My API",
        Description = "Description of My API.",
        DefaultApiVersion = "1.0"
    };

    protected override void ConfigureApiServices(IServiceCollection services)
    {
    	// 
    	// Only your custom services goes here, NOT necessary to add Mvc,
    	// Logging etc (this already done by ApiBootStartup base class).
    	// 

    	// Example:
        services.AddScoped<IMyServiceInterface, MyServiceImplementation>();
    }
}
```

# Packages included
- **OTC**
	- [Graceterm](https://github.com/OleConsignado/graceterm)
	- [Otc.Extensions.Configuration](https://github.com/OleConsignado/otc-extensions)
	- [Otc.ExceptionHandling](https://github.com/OleConsignado/otc-exception-handling) 
	- [Otc.Mvc.Filters](https://github.com/OleConsignado/otc-exception-handling)
	- [Otc.SwaggerSchemaFiltering](https://github.com/OleConsignado/otc-exception-handling)
	- [Otc.RequestTracking.AspNetCore](https://github.com/OleConsignado/otc-request-tracking)
	- [Otc.Caching.DistributedCache.All](https://github.com/OleConsignado/otc-caching)
	- [Otc.Networking.Http.Client.AspNetCore](https://github.com/OleConsignado/otc-networking)
	- [Otc.AuthorizationContext.AspNetCore.Jwt](https://github.com/OleConsignado/otc-authorization-context)
- **Microsoft**

	- [Microsoft.AspNetCore.All](https://www.nuget.org/packages/Microsoft.AspNetCore.All)
	- [Microsoft.Extensions.PlatformAbstractions](https://www.nuget.org/packages/Microsoft.Extensions.PlatformAbstractions)
	- [Microsoft.AspNetCore.Mvc.Versioning](https://www.nuget.org/packages/Microsoft.AspNetCore.Mvc.Versioning)
	- [Microsoft.AspNetCore.Mvc.Versioning.ApiExplorer](https://www.nuget.org/packages/Microsoft.AspNetCore.Mvc.Versioning.ApiExplorer)
- **Serilog**
	- [Serilog](https://www.nuget.org/packages/Serilog)
	- [Serilog.Extensions.Logging](https://www.nuget.org/packages/Serilog.Extensions.Logging)
	- [Serilog.Settings.Configuration](https://www.nuget.org/packages/Serilog.Settings.Configuration)
	- [Serilog.Sinks.Async](https://www.nuget.org/packages/Serilog.Sinks.Async)
	- [Serilog.Sinks.Console](https://www.nuget.org/packages/Serilog.Sinks.Console)
	- [Serilog.Sinks.RollingFile](https://www.nuget.org/packages/Serilog.Sinks.RollingFile)
- **Swashbuckle (Swagger)**
	- [Swashbuckle.AspNetCore](https://www.nuget.org/packages/Swashbuckle.AspNetCore)

