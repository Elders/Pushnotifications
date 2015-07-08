using System.IdentityModel.Tokens;
using Microsoft.Owin;
using Owin;
using System.Collections.Generic;
using Elders.Pandora;
using Thinktecture.IdentityServer.AccessTokenValidation;

[assembly: OwinStartup("startup", typeof(PushNotifications.Api.Startup))]
namespace PushNotifications.Api
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            log4net.Config.XmlConfigurator.Configure();

            ApplicationConfiguration.SetContext("PushNotifications");
            JwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>();
            var httpConfig = new System.Web.Http.HttpConfiguration();
            WebApiConfig.Configure(httpConfig);
            httpConfig.UseCronusCommandPublisher();
            app.UseIdentityServerBearerTokenAuthentication(new IdentityServerBearerTokenAuthenticationOptions
            {
                Authority = ApplicationConfiguration.Get("idsrv_authority")
            });
            app.UseWebApi(httpConfig);
        }
    }
}
