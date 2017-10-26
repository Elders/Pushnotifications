using System;
using System.Net;
using Elders.Pandora;
using Owin;
using PushNotifications.Api.Host.App_Start;

namespace PushNotifications.Api.Host
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            var appContext = new ApplicationContext("vapt.pushnotifications.api");
            var cfgRepo = new ConsulForPandora(new Uri("http://consul.local.com:8500"));
            var pandora = new Pandora(appContext, cfgRepo);

            LogStartup.Boot(pandora);

            var httpConfig = new System.Web.Http.HttpConfiguration();
            CronusConfig.Register(httpConfig, pandora);
            WebApiConfig.Register(httpConfig, pandora);

            app.ConfigureJwtBearerAuthentication(pandora)
               .UseWebApi(httpConfig);
        }
    }
}
