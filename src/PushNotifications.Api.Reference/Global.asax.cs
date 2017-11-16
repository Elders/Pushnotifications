using System.Web.Http;
using Elders.Web.Api;

namespace PushNotifications.Api.Reference
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            RExamplesRegistry.CollectRExamples(typeof(PushNotificationsApiAssembly).Assembly);
        }
    }
}
