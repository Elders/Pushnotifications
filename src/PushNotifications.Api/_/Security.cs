using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;

namespace PushNotifications.Api
{
    public static class SecurityServiceCollectionExtensions
    {
        public static IServiceCollection AddSecurity(this IServiceCollection services, IConfiguration configuration)
        {
            List<TenantConfigOptions> tenantConfigurations = new List<TenantConfigOptions>();
            configuration.GetRequiredSection("TenantConfig").Bind(tenantConfigurations);

            var builder = services.AddAuthentication(opt =>
            {
                opt.DefaultScheme = "AuthProvider";
                opt.DefaultChallengeScheme = "AuthProvider";
            });

            foreach (TenantConfigOptions tenantConfig in tenantConfigurations)
            {
                builder = builder.AddTenantJwtBearer(tenantConfig.Name, tenantConfig.JwtBearerOptions);
            }

            builder.AddJwtBearer();
            builder.AddPolicyScheme("AuthProvider", "AuthProvider", opt =>
            {
                opt.ForwardDefaultSelector = context =>
                {
                    string authorization = context.Request.Headers[HeaderNames.Authorization];
                    if (string.IsNullOrEmpty(authorization) == false && authorization.StartsWith("Bearer ", System.StringComparison.OrdinalIgnoreCase))
                    {
                        var token = authorization.Substring("Bearer ".Length).Trim();
                        var jwtHandler = new JwtSecurityTokenHandler();

                        if (jwtHandler.CanReadToken(token))
                        {
                            var payload = jwtHandler.ReadJwtToken(token).Payload;
                            if (payload.TryGetValue("tenant", out object tenant) && payload.TryGetValue("iss", out object authority))
                                return GetAuthenticationScheme(tenant.ToString(), authority.ToString());
                        }
                    }

                    return JwtBearerDefaults.AuthenticationScheme;
                };
            });

            return services;
        }

        private static AuthenticationBuilder AddTenantJwtBearer(this AuthenticationBuilder authenticationBuilder, string tenant, IEnumerable<JwtBearerOptions> jwtBearerOptions)
        {
            foreach (var item in jwtBearerOptions)
            {
                authenticationBuilder.AddJwtBearer(GetAuthenticationScheme(tenant, item.Authority), o =>
                {
                    o.Authority = item.Authority;
                    o.Audience = item.Audience;
                    o.RequireHttpsMetadata = item.RequireHttpsMetadata;
                });
            }

            return authenticationBuilder;
        }

        private static string GetAuthenticationScheme(string tenant, string authority) => $"{tenant}_{authority}";

        public class TenantConfigOptions
        {
            public string Name { get; set; }
            public IEnumerable<JwtBearerOptions> JwtBearerOptions { get; set; }
        }
    }
}
