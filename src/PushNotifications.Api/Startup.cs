using Microsoft.Owin;
using Owin;
using Elders.Pandora;
using System;

[assembly: OwinStartup("startup", typeof(PushNotifications.Api.Startup))]
namespace PushNotifications.Api
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            log4net.Config.XmlConfigurator.Configure();

            var appContext = new ApplicationContext("PushNotifications");
            var cfgRepo = new ConsulForPandora(new Uri("http://consul.local.com:8500"));
            var pandora = new Pandora(appContext, cfgRepo);

            app.UseHttpWebApi(pandora);
        }
    }
}
