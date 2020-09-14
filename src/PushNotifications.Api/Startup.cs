using Consul;
using Elders.Cronus;
using Elders.Cronus.AspNetCore;
using Elders.Discovery;
using Elders.Discovery.Consul;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PushNotifications.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry();
            services.AddOptions();
            services.AddLogging();

            services.AddControllers();
            services.AddScoped<ApiContext>();
            services.AddHttpContextAccessor();
            services.AddCustomProblemDetails();

            services
                .AddMvc(o =>
                {
                    o.Conventions.Add(new AddAuthorizeFiltersControllerConvention("global-scope"));
                });

            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                string authority = Configuration["idsrv_authority"];
                o.Authority = authority;
                o.Audience = authority + "/resources";
                o.RequireHttpsMetadata = true;
            });

            services.AddDiscovery(Configuration);

            services.AddCronus(Configuration);
            services.AddCronusAspNetCore();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSerilogRequestLogging();

            app.UseApiDiscovery();

            app.UseCronusAspNetCore();

            app.UseHttpsRedirection();

            app.ConfigureCors();

            app.UseRouting();

            app.UseAuthorization();
            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapDefaultControllerRoute();
            });
        }
    }

    public static class CorsExtensions
    {
        public static IApplicationBuilder ConfigureCors(this IApplicationBuilder app)
        {
            IConfiguration configuration = app.ApplicationServices.GetRequiredService<IConfiguration>();

            if (configuration.GetValue<bool>("cors_enabled") == false) return app;

            var headers = GetArrayConfiguration(configuration, "cors_headers");
            var methods = GetArrayConfiguration(configuration, "cors_methods");
            var origins = GetArrayConfiguration(configuration, "cors_origins");

            app.UseCors(builder =>
            {
                if (headers is null == false) builder.WithHeaders(headers);
                if (methods is null == false) builder.WithMethods(methods);
                if (origins is null == false) builder.WithOrigins(origins);
            });

            return app;
        }


        private static string[] GetArrayConfiguration(IConfiguration configuration, string parameter)
        {
            if (configuration is null) throw new ArgumentNullException(nameof(configuration));

            var result = new List<string>();

            string values = configuration[parameter];
            if (string.IsNullOrEmpty(values) == false)
            {
                var singleValue = values
                    .Replace("[", "").Replace("]", "").Replace("\"", "")
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(@string =>
                    {

                        return @string;
                    })
                    .Distinct();

                result.AddRange(singleValue);
            }

            return result.ToArray();
        }
    }

    public static class ApiDiscoveryExtensions
    {
        public static IApplicationBuilder UseApiDiscovery(this IApplicationBuilder builder)
        {
            var discovery = builder.ApplicationServices.GetRequiredService<ConsulRegistrationService>();
            discovery.RegisterDiscoveredEndpointsAsync().Wait();

            return builder;
        }

        public static IServiceCollection AddDiscovery(this IServiceCollection container, IConfiguration configuration)
        {
            string boundedContext = configuration["Cronus:BoundedContext"];
            string baseUrl = configuration["ApiAddress"];

            container.AddSingleton(provider => new ConsulClient(x => x.Address = ConsulHelper.DefaultConsulUri));
            container.AddSingleton<IEndpointDiscovery, AspNetCoreEndpointDiscovery>(provider =>
                new AspNetCoreEndpointDiscovery(provider.GetRequiredService<IApiDescriptionGroupCollectionProvider>(), typeof(Startup).Assembly, boundedContext, new Uri(baseUrl)));

            container.AddSingleton<ConsulRegistrationService>();
            container.AddSingleton<IDiscoveryReader, ConsulDiscoveryReader>();

            return container;
        }
    }

    public class AspNetCoreEndpointDiscovery : IEndpointDiscovery
    {
        private readonly IApiDescriptionGroupCollectionProvider apiExplorer;
        private readonly Assembly assembly;
        private readonly string boundedContext;
        private readonly Uri boundedContextBaseUri;

        public AspNetCoreEndpointDiscovery(IApiDescriptionGroupCollectionProvider apiExplorer, Assembly assembly, string boundedContext, Uri boundedContextBaseUri)
        {
            if (apiExplorer is null) throw new ArgumentNullException(nameof(apiExplorer));
            if (assembly is null) throw new ArgumentNullException(nameof(assembly));
            if (boundedContext is null) throw new ArgumentNullException(nameof(boundedContext));
            if (boundedContextBaseUri is null) throw new ArgumentNullException(nameof(boundedContextBaseUri));

            this.apiExplorer = apiExplorer;
            this.assembly = assembly;
            this.boundedContext = boundedContext;
            this.boundedContextBaseUri = boundedContextBaseUri;
        }

        public IEnumerable<DiscoverableEndpoint> Discover()
        {
            List<ControllerActionDescriptor> apiDescriptors = apiExplorer
                .ApiDescriptionGroups.Items.SelectMany(x => x.Items)
                .Where(api => HttpMethods.IsHead(api.HttpMethod) == false)
                .Select(api => api.ActionDescriptor as ControllerActionDescriptor)
                .Where(descriptor => (descriptor is null) == false)
                .ToList();

            var methodsWithDiscoverableAttribute = assembly.GetTypes()
              .SelectMany(t => t.GetMethods())
              .Where(m => m.GetCustomAttributes(typeof(DiscoverableAttribute), false).Length > 0)
              .Select(m => new { Method = m, Attr = (DiscoverableAttribute)m.GetCustomAttribute(typeof(DiscoverableAttribute), false) });

            foreach (var discoverable in methodsWithDiscoverableAttribute)
            {
                var method = discoverable.Method;
                var attr = discoverable.Attr;

                var endpoint = new DiscoverableEndpoint(attr.EndpointName, new Uri(boundedContextBaseUri, GetUrl(apiDescriptors, method)), boundedContext, new DiscoveryVersion(attr.Version, attr.DepricateVersion));
                yield return endpoint;
            }
        }

        string GetUrl(List<ControllerActionDescriptor> apiDescriptors, MethodInfo method)
        {
            var route = apiDescriptors
                .Where(descriptor => descriptor.ActionName == method.Name && descriptor.ControllerTypeInfo.AsType() == method.DeclaringType)
                .Select(descriptor => descriptor.AttributeRouteInfo.Template)
                .SingleOrDefault();

            return route;
        }
    }
}
