using System;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
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
                .ConfigureCors(pandora);
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
            //config.Filters.Add(new HostAuthenticationAttribute("Bearer"));
            //config.Filters.Add(new AuthorizeAttribute());

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

            //var converters = typeof(TimestampConverterDateTime).Assembly.GetTypes()
            //    .Where(x => typeof(JsonConverter).IsAssignableFrom(x) && x.IsAbstract == false);

            //foreach (var item in converters)
            //{
            //    settings.Converters.Add(Activator.CreateInstance(item) as JsonConverter);
            //}

            return config;
        }
    }
}
