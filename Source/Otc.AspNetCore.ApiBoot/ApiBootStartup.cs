using Graceterm;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.OpenApi.Models;
using Otc.AuthorizationContext.AspNetCore.Jwt;
using Otc.Caching.DistributedCache.All;
using Otc.Extensions.Configuration;
using Otc.Mvc.Filters;
using Otc.RequestTracking.AspNetCore;
using Otc.SwaggerSchemaFiltering;
using Serilog;
using Serilog.Exceptions;
using Serilog.Formatting.Json;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Otc.AspNetCore.ApiBoot
{
    public abstract class ApiBootStartup
    {
        protected abstract ApiMetadata ApiMetadata { get; }

        private static readonly string xmlCommentsFilePath =
            Path.Combine(PlatformServices.Default.Application.ApplicationBasePath,
                $"{PlatformServices.Default.Application.ApplicationName}.xml");
        private static readonly string buildIdFilePath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath,
                "buildid");


        protected ApiBootStartup(IConfiguration configuration)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            ApiBootOptions = Configuration.SafeGet<ApiBootOptions>();
        }

        public IConfiguration Configuration { get; }

        public ApiBootOptions ApiBootOptions { get; }

        private string BuildId
        {
            get
            {
                if (File.Exists(buildIdFilePath))
                {
                    var content = File.ReadAllText(buildIdFilePath);

                    return content?.Trim();
                }

                return "n/a";
            }
        }

        private OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
        {
            var info = new OpenApiInfo()
            {
                Title = $"{ApiMetadata.Name} (Build {BuildId})",
                Version = description.ApiVersion.ToString(),
                Description = ApiMetadata.Description
            };

            if (description.IsDeprecated)
            {
                info.Description += " Esta versão da API foi depreciada.";
            }

            return info;
        }

        private void ConfigureSwaggerAndApiVersioningServices(IServiceCollection services)
        {
            // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
            // note: the specified format code will format the version as "'v'major[.minor][-status]"
            services.AddVersionedApiExplorer(
                options =>
                {
                    options.GroupNameFormat = "'v'VVV";

                    // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                    // can also be used to control the format of the API version in route templates
                    options.SubstituteApiVersionInUrl = true;
                });

            services.AddApiVersioning(o =>
            {
                o.ReportApiVersions = true;
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = ApiVersion.Parse(ApiMetadata.DefaultApiVersion);
            });

            if (ApiBootOptions.EnableSwagger)
            {
                services.AddSwaggerGen(
                    options =>
                    {
                        // Autenticacao
                        var security = new OpenApiSecurityRequirement()
                        {
                            {
                                new OpenApiSecurityScheme()
                                {
                                    Reference = new OpenApiReference()
                                    {
                                        Type = ReferenceType.SecurityScheme,
                                        Id = "Bearer"
                                    },
                                    In = ParameterLocation.Header
                                },
                                new List<string>()
                            }
                        };

                        options.AddSecurityDefinition(
                            "Bearer",
                            new OpenApiSecurityScheme()
                            {
                                In = ParameterLocation.Header,
                                Description = "Please insert JWT with Bearer into field",
                                Name = "Authorization",
                                Type = SecuritySchemeType.ApiKey
                            });

                        options.AddSecurityRequirement(security);

                        // Filtro referente ao mecanismo de tratamento de excecoes (Otc.ExceptionHandling):
                        // Remove diversas propriedades do tipo Exception para o schema do swagger
                        options.ApplyOtcDomainBaseExceptionSchemaFilter();

                        // note: that we have to build a temporary service provider here because one
                        //has not been created yet
                        var tempServiceProvider = services.BuildServiceProvider();

                        // resolve the IApiVersionDescriptionProvider service
                        var apiVersionDescriptionProvider = tempServiceProvider.
                            GetRequiredService<IApiVersionDescriptionProvider>();

                        // add a swagger document for each discovered API version
                        // note: you might choose to skip or document deprecated API versions differently
                        foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
                        {
                            options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
                        }

                        // add a custom operation filter which sets default values
                        options.OperationFilter<SwaggerDefaultValues>();

                        if (File.Exists(xmlCommentsFilePath))
                        {
                            // integrate xml comments
                            options.IncludeXmlComments(xmlCommentsFilePath);
                        }
                        else
                        {
                            Log.Logger.Warning("Could not read Xml comments file, path '{XmlCommentsFilePath}' " +
                                "not exists.", xmlCommentsFilePath);
                        }
                    });
            }
        }

        private string requestTrackDisableBodyCapturingForUrl;

        /// <summary>
        /// Url to log request information but dont capture body (case insensitive regex pattern). 
        /// <para>
        /// Place to put authentication urls in order to prevent capture of credentials.
        /// </para>
        /// <para>
        /// Only the portion after host/port, including querystring will be analyzed, in other words, 
        /// the path + querystring.
        /// </para>
        /// </summary>
        protected void RequestTrackingDisableBodyCapturingForUrl(string pathRegexPattern)
        {
            requestTrackDisableBodyCapturingForUrl = pathRegexPattern;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddAspNetCoreHttpClientFactoryWithCorrelation();
            services.AddControllers();
            ConfigureLogging(services);
            services.AddOtcAspNetCoreJwtAuthorizationContext(Configuration.SafeGet<JwtConfiguration>());
            ConfigureMvc(services);
            ConfigureSwaggerAndApiVersioningServices(services);
            services.AddExceptionHandling();
            ConfigureRequestTracker(services);
            services.AddOtcDistributedCache(Configuration.SafeGet<DistributedCacheConfiguration>());
            ConfigureApiServices(services);
        }

        private void ConfigureRequestTracker(IServiceCollection services)
        {
            var requestTrackerConfiguration = Configuration.SafeGet<RequestTrackerConfiguration>();

            if (string.IsNullOrEmpty(requestTrackerConfiguration.ExcludeUrl))
            {
                requestTrackerConfiguration.ExcludeUrl = Regex.Escape(HealthChecksController.RoutePath);
            }
            else
            {
                requestTrackerConfiguration.ExcludeUrl =
                    $"({requestTrackerConfiguration.ExcludeUrl})|({Regex.Escape(HealthChecksController.RoutePath)})";
            }

            if (!string.IsNullOrEmpty(requestTrackDisableBodyCapturingForUrl))
            {
                if (string.IsNullOrEmpty(requestTrackerConfiguration.DisableBodyCapturingForUrl))
                {
                    requestTrackerConfiguration.DisableBodyCapturingForUrl = requestTrackDisableBodyCapturingForUrl;
                }
                else
                {
                    requestTrackerConfiguration.DisableBodyCapturingForUrl =
                        $"({requestTrackerConfiguration.DisableBodyCapturingForUrl})|" +
                        $"({requestTrackDisableBodyCapturingForUrl})";
                }
            }

            services.AddRequestTracking(requestTracker =>
            {
                requestTracker.Configure(requestTrackerConfiguration);
            });
        }

        private void ConfigureMvc(IServiceCollection services)
        {
            services.AddMvc(options =>
            {
                options.Filters.Add<ExceptionFilter>();
                ConfigureMvcOptions(options);
            }).AddNewtonsoftJson()
            .AddJsonOptions(options =>
            {
                if (ApiBootOptions.EnableStringEnumConverter)
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                }

                options.JsonSerializerOptions.IgnoreNullValues = true;
            });
        }

        private void ConfigureLogging(IServiceCollection services)
        {
            services.AddLogging(configure =>
            {
                configure.ClearProviders();

                if (ApiBootOptions.EnableLogging)
                {
                    var loggerConfiguration = new LoggerConfiguration()
                        .ReadFrom.Configuration(Configuration)
                        .Enrich.WithExceptionDetails();

                    if (ApiBootOptions.LoggingType != LoggingType.SerilogRawConfiguration)
                    {
                        loggerConfiguration = loggerConfiguration
                            .Enrich.FromLogContext()
                            .Enrich.WithProcessId()
                            .Enrich.WithProcessName()
                            .Enrich.WithThreadId()
                            .Enrich.WithEnvironmentUserName()
                            .Enrich.WithMachineName();

                        if (ApiBootOptions.LoggingType == LoggingType.ApiBootFile)
                            loggerConfiguration = loggerConfiguration.WriteTo
                                .Async(a => a.File($"logs/log-.txt", rollingInterval: RollingInterval.Day));
                        else
                            loggerConfiguration = loggerConfiguration.WriteTo
                                .Async(a => a.Console(new JsonFormatter()));
                    }

                    Log.Logger = loggerConfiguration.CreateLogger();

                    configure.AddSerilog();
                }
            });
        }

        public virtual void ConfigureMvcOptions(MvcOptions options) { }
        
        protected abstract void ConfigureApiServices(IServiceCollection services);

        public void Configure(IApplicationBuilder app, IApiVersionDescriptionProvider apiVersionDescriptionProvider)
        {
            //app.UseRequestTracking();

            app.UseGraceterm(options =>
            {
                options.IgnorePath(HealthChecksController.RoutePath);
            });

            app.UseApiVersioning();
            app.UseAuthentication();
            app.UseBuildIdTracker(BuildId);
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            if (ApiBootOptions.EnableSwagger)
            {
                app.UseSwagger();
                app.UseSwaggerUI(
                    options =>
                    {
                        // build a swagger endpoint for each discovered API version
                        foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
                        {
                            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                                description.GroupName.ToUpperInvariant());
                        }
                    });
            }
        }
    }
}
