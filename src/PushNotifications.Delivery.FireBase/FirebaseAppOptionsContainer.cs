using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace PushNotifications.Delivery.FireBase
{
    public sealed class FirebaseAppOptionsContainer
    {
        private IReadOnlyCollection<FirebaseAppOption> optionsCollection;

        public FirebaseAppOptionsContainer(IOptions<FireBaseOptions> opt)
        {
            optionsCollection = InitializeAppOptions(opt.Value);
        }

        public AppOptions GetAppOptions(string tenant, string application)
        {
            if (string.IsNullOrEmpty(application))
            {
                return optionsCollection.Where(x => x.Tenant.Equals(tenant, System.StringComparison.OrdinalIgnoreCase) && x.Application.Equals("vapt", System.StringComparison.OrdinalIgnoreCase))
                                        .SingleOrDefault()?.Options;
            }
            else
            {
                return optionsCollection.Where(x => x.Tenant.Equals(tenant, System.StringComparison.OrdinalIgnoreCase) && x.Application.Equals(application, System.StringComparison.OrdinalIgnoreCase))
                                        .SingleOrDefault()?.Options;
            }

        }

        private IReadOnlyCollection<FirebaseAppOption> InitializeAppOptions(FireBaseOptions options)
        {
            var collection = new List<FirebaseAppOption>();

            foreach (FireBaseOptions.AuthorizationKey opt in options.Authorizations)
            {
                AppOptions appOptions = new AppOptions()
                {
                    Credential = GoogleCredential.FromJson(JsonSerializer.Serialize(opt.ServiceAccountOptions))
                };

                FirebaseAppOption appOpt = new FirebaseAppOption(opt.Tenant, opt.Application, appOptions);
                collection.Add(appOpt);
            }

            return collection.AsReadOnly();
        }
    }
}
