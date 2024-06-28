using Microsoft.Extensions.Configuration;
using OptionsExtensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PushNotifications.Delivery.FireBase
{
    public class FireBaseOptions
    {
        public string BaseAddress { get; set; }

        public List<AuthorizationKey> Authorizations { get; set; }

        public class AuthorizationKey
        {
            public string Tenant { get; set; }

            public string Application { get; set; }

            public ServiceAccountOptions ServiceAccountOptions { get; set; }
        }

        public class ServiceAccountOptions
        {
            public string type { get; set; }
            public string project_id { get; set; }
            public string private_key_id { get; set; }
            public string private_key { get; set; }
            public string client_id { get; set; }
            public string client_email { get; set; }
            public string auth_uri { get; set; }
            public string token_uri { get; set; }
            public string auth_provider_x509_cert_url { get; set; }
            public string client_x509_cert_url { get; set; }
            public string universe_domain { get; set; }
        }

        [Obsolete]
        public AuthorizationKey GetKey(string tenant, string application)
        {
            if (string.IsNullOrEmpty(application))
            {
                return Authorizations.Where(x => x.Tenant.Equals(tenant, System.StringComparison.OrdinalIgnoreCase) && x.Application.Equals("vapt", System.StringComparison.OrdinalIgnoreCase)).SingleOrDefault();
            }
            else
            {
                return Authorizations.Where(x => x.Tenant.Equals(tenant, System.StringComparison.OrdinalIgnoreCase) && x.Application.Equals(application, System.StringComparison.OrdinalIgnoreCase)).SingleOrDefault();
            }
        }
    }

    public class FireBaseOptionsProvider : OptionsProviderBase<FireBaseOptions>
    {
        public const string SettingKey = "FireBase";

        public FireBaseOptionsProvider(IConfiguration configuration) : base(configuration) { }

        public override void Configure(FireBaseOptions options)
        {
            configuration.GetSection(SettingKey).Bind(options);
        }
    }
}
