using Elders.Web.Api.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Cors;
using Elders.Cronus.IocContainer;
using Elders.Cronus.DomainModeling;
using System.Web.Http.Dispatcher;

namespace PushNotifications.Api
{
    public static class WebApiConfig
    {
        public static void Configure(HttpConfiguration apiConfig)
        {
            if (apiConfig == null) throw new ArgumentNullException("apiConfig");
            apiConfig.Filters.Add(new VerifyModelState());
            apiConfig.Filters.Add(new ExceptionFilter());

            apiConfig.MapHttpAttributeRoutes();
            apiConfig.SuppressDefaultHostAuthentication();
            apiConfig.Filters.Add(new HostAuthenticationAttribute("Bearer"));

            var cors = new EnableCorsAttribute("*", "*", "*");
            apiConfig.EnableCors(cors);

            apiConfig.Filters.Add(new AuthorizeAttribute());

            apiConfig.Formatters.Remove(apiConfig.Formatters.XmlFormatter);
            apiConfig.Formatters.Remove(apiConfig.Formatters.FormUrlEncodedFormatter);

            JsonSerializerSettings settings = apiConfig.Formatters.JsonFormatter.SerializerSettings;

            settings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Include;
            settings.Formatting = Formatting.Indented;
            settings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
            settings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            settings.Converters.Add(new ErrorConverter());
        }
    }

    public class CustomHttpControllerActivator : IHttpControllerActivator
    {
        IContainer container;

        public CustomHttpControllerActivator(Elders.Cronus.IocContainer.IContainer container)
        {
            this.container = container;
        }

        public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType)
        {
            var controller = Activator.CreateInstance(controllerType) as IHttpController;
            var publisherProp = controllerType.GetProperty("Publisher");
            if (publisherProp != null)
                publisherProp.SetValue(controller, container.Resolve<IPublisher<ICommand>>());
            return controller;
        }
    }
}
