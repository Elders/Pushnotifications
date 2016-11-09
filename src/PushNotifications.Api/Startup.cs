using System.IdentityModel.Tokens;
using Microsoft.Owin;
using Owin;
using System.Collections.Generic;
using Elders.Pandora;
using IdentityServer3.AccessTokenValidation;
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

            JwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>();
            var httpConfig = new System.Web.Http.HttpConfiguration();
            WebApiConfig.Configure(httpConfig);
            httpConfig.UseCronusCommandPublisher(pandora);
            app.UseIdentityServerBearerTokenAuthentication(new IdentityServerBearerTokenAuthenticationOptions
            {
                Authority = pandora.Get("idsrv_authority")
            });
            app.UseWebApi(httpConfig);
        }
    }
}
