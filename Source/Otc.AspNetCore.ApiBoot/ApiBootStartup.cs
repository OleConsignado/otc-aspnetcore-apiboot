using Graceterm;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Otc.AuthorizationContext.AspNetCore.Jwt;
using Otc.Caching.DistributedCache.All;
using Otc.Extensions.Configuration;
using Otc.Mvc.Filters;
using Otc.RequestTracking.AspNetCore;
using Otc.SwaggerSchemaFiltering;
using Serilog;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Otc.AspNetCore.ApiBoot
{
    public abstract class ApiBootStartup
    {
        protected abstract ApiMetadata ApiMetadata { get; }

        private static readonly string xmlCommentsFilePath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, $"{PlatformServices.Default.Application.ApplicationName}.xml");

        protected ApiBootStartup(IConfiguration configuration)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            ApiBootOptions = Configuration.SafeGet<ApiBootOptions>();
        }

        public IConfiguration Configuration { get; }

        public ApiBootOptions ApiBootOptions { get; }

        private Info CreateInfoForApiVersion(ApiVersionDescription description)
        {
            var info = new Info()
            {
                Title = $"{ApiMetadata.Name} {description.ApiVersion}",
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
                        options.DescribeAllEnumsAsStrings();

                        // Autenticacao
                        var security = new Dictionary<string, IEnumerable<string>>
                            {
                                { "Bearer", new string[] { } }
                            };

                        options.AddSecurityDefinition(
                            "Bearer",
                            new ApiKeyScheme()
                            {
                                In = "header",
                                Description = "Please insert JWT with Bearer into field",
                                Name = "Authorization",
                                Type = "apiKey"
                            });

                        options.AddSecurityRequirement(security);

                        // Filtro referente ao mecanismo de tratamento de excecoes (Otc.ExceptionHandling):
                        // Remove diversas propriedades do tipo Exception para o schema do swagger
                        options.SchemaFilter<SwaggerExcludeFilter>();


                        // note: that we have to build a temporary service provider here because one has not been created yet
                        var tempServiceProvider = services.BuildServiceProvider();

                        // resolve the IApiVersionDescriptionProvider service
                        var apiVersionDescriptionProvider = tempServiceProvider.GetRequiredService<IApiVersionDescriptionProvider>();

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
                            Log.Logger.Warning("Could not read Xml comments file, path '{XmlCommentsFilePath}' not exists.", xmlCommentsFilePath);
                        }
                    });
            }
        }

        private string _requestTrackDisableBodyCapturingForUrl;

        /// <summary>
        /// Url to log request information but dont capture body (case insensitive regex pattern). 
        /// <para>
        /// Place to put authentication urls in order to prevent capture of credentials.
        /// </para>
        /// <para>
        /// Only the portion after host/port, including querystring will be analyzed, in other words, the path + querystring.
        /// </para>
        /// </summary>
        protected void RequestTrackingDisableBodyCapturingForUrl(string pathRegexPattern)
        {
            _requestTrackDisableBodyCapturingForUrl = pathRegexPattern;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddAspNetCoreHttpClientFactoryWithCorrelation();

            services.AddLogging(configure =>
            {
                configure.ClearProviders();

                if (ApiBootOptions.EnableLogging)
                {
                    ValidateSerilogConfiguration();

                    Log.Logger = new LoggerConfiguration()
                        .ReadFrom.Configuration(Configuration)
                        .CreateLogger();

                    configure.AddSerilog();
                    configure.AddDebug();
                }
            });

            services.AddOtcAspNetCoreJwtAuthorizationContext(Configuration.SafeGet<JwtConfiguration>());

            services.AddMvc(options =>
            {
                options.Filters.Add<ExceptionFilter>();
            });

            ConfigureSwaggerAndApiVersioningServices(services);

            services.AddExceptionHandling();

            var requestTrackerConfiguration = Configuration.SafeGet<RequestTrackerConfiguration>();

            if(string.IsNullOrEmpty(requestTrackerConfiguration.ExcludeUrl))
            {
                requestTrackerConfiguration.ExcludeUrl = Regex.Escape(HealthChecksController.RoutePath);
            }
            else
            {
                requestTrackerConfiguration.ExcludeUrl = $"({requestTrackerConfiguration.ExcludeUrl})|({Regex.Escape(HealthChecksController.RoutePath)})";
            }

            if(!string.IsNullOrEmpty(_requestTrackDisableBodyCapturingForUrl))
            {
                if (string.IsNullOrEmpty(requestTrackerConfiguration.DisableBodyCapturingForUrl))
                {
                    requestTrackerConfiguration.DisableBodyCapturingForUrl = _requestTrackDisableBodyCapturingForUrl;
                }
                else
                {
                    requestTrackerConfiguration.DisableBodyCapturingForUrl = $"({requestTrackerConfiguration.DisableBodyCapturingForUrl})|({_requestTrackDisableBodyCapturingForUrl})";
                }
            }

            services.AddRequestTracking(requestTracker =>
            {
                requestTracker.Configure(requestTrackerConfiguration);
            });

            services.AddOtcDistributedCache(Configuration.SafeGet<DistributedCacheConfiguration>());

            ConfigureApiServices(services);
        }

        protected abstract void ConfigureApiServices(IServiceCollection services);

        public void Configure(IApplicationBuilder app, IApiVersionDescriptionProvider apiVersionDescriptionProvider)
        {
            app.UseRequestTracking();

            app.UseGraceterm(options =>
            {
                options.IgnorePath(HealthChecksController.RoutePath);
            });

            app.UseApiVersioning();
            app.UseAuthentication();
            app.UseMvc();

            if (ApiBootOptions.EnableSwagger)
            {
                app.UseSwagger();
                app.UseSwaggerUI(
                    options =>
                    {
                    // build a swagger endpoint for each discovered API version
                    foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
                        {
                            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                        }
                    });
            }
        }

        private void ValidateSerilogConfiguration() => Configuration.GetSection("Serilog").SafeGet<SerilogConfiguration>();
    }
}