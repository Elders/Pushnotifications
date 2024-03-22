using Microsoft.Extensions.Configuration;
using OptionsExtensions;
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

            public string ServerKey { get; set; }
        }

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
