using System;
using Elders.Pandora;
using System.Web.Http;
using System.Reflection;
using PushNotifications.Api.Logging;
using System.Web.Http.Dispatcher;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Linq;
using Elders.Web.Api.Filters;
using Newtonsoft.Json;
using IdentityServer3.AccessTokenValidation;
using Owin;
using System.IdentityModel.Tokens;

namespace PushNotifications.Api
{
    public static class Cronus
    {
        static ILog log = LogProvider.GetLogger(typeof(Cronus));

        public static void UseHttpWebApi(this IAppBuilder app, Pandora pandora)
        {
            try
            {
                log.Info("Starting Cronus Push Notifications Api");

                JwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>();

                var apiConfig = new HttpConfiguration();
                GlobalConfigureApi(apiConfig);

                var configurations = typeof(Cronus).Assembly.GetTypes().Where(x => x.GetInterfaces().Contains(typeof(IApiConfiguration)))
                     .Select(x => Activator.CreateInstance(x) as IApiConfiguration).ToList();
                foreach (var item in configurations)
                {
                    try
                    {
                        item.Initialize(apiConfig, pandora);
                    }
                    catch (Exception ex)
                    {
                        log.FatalException($"Failed to initialize:{item.GetType().Name}", ex);
                        throw;
                    }
                }

                apiConfig.Services.Replace(typeof(IAssembliesResolver), new ControllerAssembliesResolver(configurations.Select(x => x.Assembly).ToList()));
                apiConfig.Services.Replace(typeof(IHttpControllerActivator), new CompositeLocatorFactory(configurations.ToList()));

                app.UseIdentityServerBearerTokenAuthentication(new IdentityServerBearerTokenAuthenticationOptions
                {
                    Authority = pandora.Get("idsrv_authority")
                });

                app.UseWebApi(apiConfig);

                log.Info("Started Cronus Push Notifications Api");
            }
            catch (Exception ex)
            {
                log.ErrorException("Unable to boot PushNotifications.Api", ex);
                throw;
            }
        }

        private static void GlobalConfigureApi(HttpConfiguration apiConfig)
        {
            if (apiConfig == null) throw new ArgumentNullException(nameof(apiConfig));
            apiConfig.Filters.Add(new VerifyModelState());
            apiConfig.Filters.Add(new ExceptionFilter());

            apiConfig.SuppressDefaultHostAuthentication();
            apiConfig.Filters.Add(new HostAuthenticationAttribute("Bearer"));
            apiConfig.Filters.Add(new AuthorizeAttribute());

            apiConfig.Formatters.Remove(apiConfig.Formatters.XmlFormatter);

            JsonSerializerSettings settings = apiConfig.Formatters.JsonFormatter.SerializerSettings;

            settings.DateFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.ffffff'Z'";
            settings.Converters.Add(new ErrorConverter());
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.Formatting = Formatting.Indented;
            settings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            var contractReslover = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
            settings.ContractResolver = contractReslover;
            contractReslover.DefaultMembersSearchFlags = contractReslover.DefaultMembersSearchFlags | BindingFlags.NonPublic;
        }
    }

    public interface IApiConfiguration
    {
        Assembly Assembly { get; }

        IHttpControllerActivator ControllerFactory { get; }

        void Initialize(HttpConfiguration configuration, Pandora pandora);
    }

    public class CompositeLocatorFactory : IHttpControllerActivator
    {
        List<IApiConfiguration> configurations;

        public CompositeLocatorFactory(List<IApiConfiguration> configurations)
        {
            this.configurations = configurations;
        }

        public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType)
        {
            var configuration = configurations.SingleOrDefault(x => x.Assembly == controllerType.Assembly);
            return configuration.ControllerFactory.Create(request, controllerDescriptor, controllerType);
        }
    }

    public class ControllerAssembliesResolver : DefaultAssembliesResolver
    {
        ICollection<Assembly> assemblies;

        public ControllerAssembliesResolver(ICollection<Assembly> assemblies)
        {
            this.assemblies = assemblies;
        }
        public override ICollection<Assembly> GetAssemblies()
        {
            return assemblies;
        }
    }
}
