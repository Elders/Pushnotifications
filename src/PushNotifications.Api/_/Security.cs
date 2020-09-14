using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PushNotifications.Api
{
    public class AddAuthorizeFiltersControllerConvention : IControllerModelConvention
    {
        private readonly string globalScope;

        public AddAuthorizeFiltersControllerConvention(string globalScope)
        {
            this.globalScope = globalScope;
        }

        public void Apply(ControllerModel controller)
        {
            controller.Filters.Add(new AuthorizeFilter());
        }
    }

    public static class SecurityServiceCollectionExtensions
    {
        public static IServiceCollection AddSecurity(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMvc(o => o.Conventions.Add(new AddAuthorizeFiltersControllerConvention("global-scope")));

            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                string authority = configuration["idsrv_authority"];
                o.Authority = authority;
                o.Audience = authority + "/resources";
                o.RequireHttpsMetadata = true;
            });

            return services;
        }
    }
}
