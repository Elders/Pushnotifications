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
            var appContext = new ApplicationContext("Vapt.PushNotifications.Api");
            var cfgRepo = new ConsulForPandora(new Uri("http://consul.local.com:8500"));
            var pandora = new Pandora(appContext, cfgRepo);

            LogStartup.Boot(pandora);

            app.UseWelcomePage("/");
            app.UseHttpWebApi(pandora);
        }
    }
}
