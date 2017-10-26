using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;
using Elders.Pandora;
using Microsoft.Owin.Security.Jwt;
using Owin;

namespace PushNotifications.Api.Host
{
    public static class IAppBuilderExtensions
    {
        public static IAppBuilder ConfigureJwtBearerAuthentication(this IAppBuilder app, Pandora pandora)
        {
            //  qoretIssuer = "https://account-int.marketvision.com";
            //  qoreAudience = "https://account-int.marketvision.com/resources";
            var idsrvs = pandora.Get<List<IdentityServerModel>>("idsrvs");

            if (idsrvs.Count == 0)
                return app;

            var tokenProviders = new List<X509CertificateSecurityTokenProvider>();
            var certificates = new List<X509Certificate2>();
            var issuers = new List<string>();
            var audiences = new List<string>();

            foreach (var idsrv in idsrvs)
            {
                var certBytes = Cert.GetCertBytes(idsrv.Issuer);
                var certificate = new X509Certificate2(certBytes);

                tokenProviders.Add(new X509CertificateSecurityTokenProvider(idsrv.Issuer, certificate));
                issuers.Add(idsrv.Issuer);
                audiences.Add(idsrv.Audience);
            }

            app.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
            {
                TokenValidationParameters = GetTokenValidationParameters(certificates, issuers, audiences),
                IssuerSecurityTokenProviders = tokenProviders
            });

            return app;
        }

        static TokenValidationParameters GetTokenValidationParameters(IEnumerable<X509Certificate2> certs, IEnumerable<string> issuers, IEnumerable<string> audiences)
        {
            TokenValidationParameters tvp = new TokenValidationParameters();
            tvp.ValidIssuers = issuers;
            tvp.ValidAudiences = audiences;
            tvp.ValidateIssuer = true;
            tvp.ValidateAudience = true;
            tvp.ValidateIssuerSigningKey = true;
            tvp.ValidateLifetime = true;

            var tokens = new List<SecurityToken>();
            var keys = new List<X509SecurityKey>();

            foreach (var certificate in certs)
            {
                var certToken = new X509SecurityToken(certificate);
                var key = new X509SecurityKey(certificate);

                tokens.Add(certToken);
                keys.Add(key);
            }

            tvp.IssuerSigningTokens = tokens;
            tvp.IssuerSigningKeys = keys;

            return tvp;
        }

        class IdentityServerModel
        {
            public IdentityServerModel(string issuer, string udience)
            {
                Issuer = issuer;
                this.Audience = udience;
            }

            public string Issuer { get; private set; }

            public string Audience { get; private set; }
        }
    }
}
