# Welcome to Otc.AspNetCore.ApiBoot
[![Build Status](https://travis-ci.org/OleConsignado/otc-aspnetcore-apiboot.svg?branch=master)](https://travis-ci.org/OleConsignado/otc-aspnetcore-apiboot)

The **Otc.AspNetCore.ApiBoot** (referenced here as just **ApiBoot**) goal is to reduce boilerplate while creating new *AspNet Core WebApi* projects. It brings a lot of stuff that almost all API should provide. 

So you can keep focus on the real problem you need to address instead of spend time configuring logs, authentication/authorization, swagger and more.

# Quickstart

**Install ApiBoot template**

```sh
$ dotnet new -i apiboot
``` 

**Create a new project**

```sh
$ dotnet new apiboot --name=MyProjectName
```

**Or clone** example repository https://github.com/OleConsignado/apiboot-example

# ApiBootStartup

The key element of **ApiBoot** is the `ApiBootStartup` class which was designed to be the base class of the regular *AspNet Core WebApi* `Startup` class.

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

# ApiController
Use `Otc.AspNetCore.ApiBoot.ApiController` instead of `Microsoft.AspNetCore.Mvc.ControllerBase` as base class for your controllers.

By using the `ApiController` as base class you will get:

### Route based versioning

`ApiController` defines **/vN/[ControllerName]** base route for your controller, where N is the version number defined in `ApiVersion` attribute at your controller class or `DefaultApiVersion` of `ApiMetadata` (provided at `Startup`) if the controller is not decored with `ApiVersion` attribute. 

By using `ApiController` as base class, you should not decorate your controller with `Route` attribute. If you do, it will use the given route as additional route to the already define **/vN/[ControllerName]**.

**[ControllerName]** segment will be extracted from controller class name by removing **Controller** suffix, example:

```cs
using Otc.AspNetCore.ApiBoot;
using Microsoft.AspNetCore.Mvc;

[ApiVersion("1.0")]
public class UsersController : ApiController { }
```
will get routed to **/v1/Users**.

```cs
using Otc.AspNetCore.ApiBoot;
using Microsoft.AspNetCore.Mvc;

[ApiVersion("1.0")]
[ApiVersion("2.0")]
public class UsersController : ApiController { }
```
will get routed to both **/v1/Users** and **/v2/Users**.

#### Dealing with multiple versions
```cs
using Otc.AspNetCore.ApiBoot;
using Microsoft.AspNetCore.Mvc;

[ApiVersion("1.0")]
[ApiVersion("2.0")]
public class UsersController : ApiController 
{ 
    [HttpGet, MapToApiVersion("1.0")] // GET /v1/Users
    public IActionResult GetV1() { ... }

    [HttpGet, MapToApiVersion("2.0")] // GET /v2/Users
    public IActionResult GetV2() { ... }

    [HttpPost] // The both POST /v1/Users and POST /v2/Users
    public IActionResult PostVersionInvariant() { ... }
}
```
- GET **/v1/Users** will get routed to `GetV1` method;
- GET **/v2/Users** will get routed to `GetV2` method;
- POST **/v1/Users** will get routed to `PostVersionInvariant` method;
- POST **/v2/Users** will get routed to `PostVersionInvariant` method.

### Authorization

The `ApiController` base class is decorated with `Authorize` attribute (from `Microsoft.AspNetCore.Authorization`) what means that your controller class, derived from `ApiController` will inherit this definition. 

This will make all requests requires a valid authorization creedentials or a 401 HTTP error code will get as response and the controller method will not invoked.

To bypass authorization checks, you need to decorate every method you wants to expose as anonymous requests with the `AllowAnonymous` attribute (from `Microsoft.AspNetCore.Authorization`).

**Example:**

```cs
using Otc.AspNetCore.ApiBoot;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiVersion("1.0")]
public class AutheticationController : ApiController 
{ 
    [HttpPost, AllowAnonymous]
    public IActionResult Post(UserCreedentials creedentials) { ... }
}
```

> **Note:** If you wondering on how to deal with authentication/authorization, you must know that the **APIBoot** is absolutely compatible with raw AspNetCore authorization mechanism, so you may follow up the [Microsoft Docs](https://docs.microsoft.com/aspnet/core/security/authorization/?view=aspnetcore-2.1) to learn more **OR** if you interested in a more convenient way to handle this, you should use the [Otc.AuthorizationContext](https://github.com/OleConsignado/otc-authorization-context) library (recommended) which is already included in **ApiBoot**.

# Included packages

The packages listed bellow are all included on **ApiBoot** as it dependencies, so by using **ApiBoot** you don't need to add reference to the these packages in your project. 

- **OTC** ([Olé](https://github.com/OleConsignado))
	- [Graceterm](https://github.com/OleConsignado/graceterm)
	- [Otc.Caching.DistributedCache.All](https://github.com/OleConsignado/otc-caching)	
	- [Otc.Extensions.Configuration](https://github.com/OleConsignado/otc-extensions)
	- [Otc.ExceptionHandling](https://github.com/OleConsignado/otc-exception-handling) 
		- Otc.ExceptionHandling
		- Otc.Mvc.Filters
		- Otc.SwaggerSchemaFiltering
	- [Otc.RequestTracking.AspNetCore](https://github.com/OleConsignado/otc-request-tracking)
	- [Otc.Networking.Http.Client.AspNetCore](https://github.com/OleConsignado/otc-networking)
	- [Otc.AuthorizationContext.AspNetCore.Jwt](https://github.com/OleConsignado/otc-authorization-context)
	- [Otc.RequestTracking](https://github.com/OleConsignado/otc-request-tracking)
- **Microsoft**

	- [Microsoft.AspNetCore.All](https://www.nuget.org/packages/Microsoft.AspNetCore.All)
	- [Microsoft.Extensions.PlatformAbstractions](https://www.nuget.org/packages/Microsoft.Extensions.PlatformAbstractions)
	- [Microsoft.AspNetCore.Mvc.Versioning](https://www.nuget.org/packages/Microsoft.AspNetCore.Mvc.Versioning)
	- [Microsoft.AspNetCore.Mvc.Versioning.ApiExplorer](https://www.nuget.org/packages/Microsoft.AspNetCore.Mvc.Versioning.ApiExplorer)
- **Serilog**
    - [Serilog](https://www.nuget.org/packages/Serilog)
    - [Serilog.Enrichers.Environment](https://www.nuget.org/packages/Serilog.Enrichers.Environment)
    - [Serilog.Enrichers.Process](https://www.nuget.org/packages/Serilog.Enrichers.Process)
    - [Serilog.Enrichers.Thread](https://www.nuget.org/packages/Serilog.Enrichers.Thread)
    - [Serilog.Exceptions](https://www.nuget.org/packages/Serilog.Exceptions)
    - [Serilog.Extensions.Logging](https://www.nuget.org/packages/Serilog.Extensions.Logging)
    - [Serilog.Settings.Configuration](https://www.nuget.org/packages/Serilog.Settings.Configuration)
    - [Serilog.Sinks.Async](https://www.nuget.org/packages/Serilog.Sinks.Async)
    - [Serilog.Sinks.Console](https://www.nuget.org/packages/Serilog.Sinks.Console)
    - [serilog.Sinks.File](https://www.nuget.org/packages/Serilog.Sinks.File)
- **Swashbuckle (Swagger)**
	- [Swashbuckle.AspNetCore](https://www.nuget.org/packages/Swashbuckle.AspNetCore)


> **Note:** If you created a project from the standard template, **Microsoft** packages comes as reference by default, so, in this case, it's recomended to remove these packages once it's all already referenced by `Otc.AspNetCore.ApiBoot`. By removing these package will also avoid version conflicts issues.
