using System;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using Consul;
using Discovery.Consul;
using Discovery.Contracts;
using Elders.Cronus.DomainModeling;
using Elders.Pandora;
using Elders.Web.Api.Filters;
using Newtonsoft.Json;

namespace PushNotifications.Api.Host.App_Start
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config, Pandora pandora)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            config
                .ConfigureRoutes()
                .ConfigureFilters()
                .ConfigureJsonSerializer()
                .ConfigureCors(pandora)
                .RegisterServices(pandora);
        }

        static HttpConfiguration ConfigureCors(this HttpConfiguration config, Pandora pandora)
        {
            if (pandora.Get<bool>("cors_enabled"))
            {
                var origins = pandora.Get("cors_origins");
                var headers = pandora.Get("cors_headers");
                var methods = pandora.Get("cors_methods");

                config.EnableCors(new EnableCorsAttribute(origins, headers, methods));
            }

            return config;
        }

        static HttpConfiguration ConfigureRoutes(this HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            return config;
        }

        static HttpConfiguration ConfigureFilters(this HttpConfiguration config)
        {
            config.Filters.Add(new VerifyModelState());
            config.Filters.Add(new ExceptionFilter());

            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationAttribute("Bearer"));
            config.Filters.Add(new AuthorizeAttribute());

            config.Formatters.Remove(config.Formatters.XmlFormatter);
            config.Formatters.Remove(config.Formatters.FormUrlEncodedFormatter);

            return config;
        }

        static HttpConfiguration ConfigureJsonSerializer(this HttpConfiguration config)
        {
            JsonSerializerSettings settings = config.Formatters.JsonFormatter.SerializerSettings;
            settings.Converters.Add(new ErrorConverterWithExpandedErrorMessage(() => HttpContext.Current.GetOwinContext()));
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.Formatting = Formatting.Indented;
            settings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
            settings.ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor;
            settings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            settings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;

            var converters = typeof(PushNotificationsApiAssembly).Assembly.GetTypes()
                .Where(x => typeof(JsonConverter).IsAssignableFrom(x) && x.IsAbstract == false);

            foreach (var item in converters)
            {
                settings.Converters.Add(Activator.CreateInstance(item) as JsonConverter);
            }

            return config;
        }

        static HttpConfiguration RegisterServices(this HttpConfiguration config, Pandora pandora)
        {
            config.EnsureInitialized();
            var baseUri = new Uri(pandora.Get("pn_base_url"));
            var httpHealthCheckUri = new Uri(pandora.Get("pn_health_check_url"));
            var consulClient = new ConsulClient(x => x.Address = ConsulHelper.ConsulUri);
            var consulRegistrationService = new ConsulRegistrationService(consulClient);
            consulRegistrationService.UnRegisterServices(typeof(PushNotificationsApiAssembly).Assembly.GetBoundedContext().BoundedContextName);
            //RegisterServices(config, typeof(PushNotificationsApiAssembly).Assembly, baseUri);
            consulRegistrationService.RegisterServices(config, typeof(PushNotificationsApiAssembly).Assembly, baseUri);
            consulRegistrationService.RegisterService("pn", httpHealthCheckUri);

            return config;
        }
    }
}
