using System;
using PushNotifications.Api.Client.Infrastructure;
using PushNotifications.Api.Client.Logging;

namespace PushNotifications.Api.Client
{
    public sealed partial class PushNotificationsClient
    {
        static ILog log = LogProvider.GetLogger(typeof(PushNotificationsClient));

        Options options;
        Authenticator authenticator;

        public PushNotificationsClient(Options options, Authenticator authenticator = null)
        {
            if (ReferenceEquals(null, options)) throw new ArgumentNullException(nameof(options));
            this.options = options;

            this.authenticator = authenticator;
        }

        public sealed class Options
        {
            public Options(Uri apiAddress)
            {
                ApiAddress = apiAddress;
                JsonSerializer = NewtonsoftJsonSerializer.Default();
            }

            public Uri ApiAddress { get; private set; }
            public IJsonSerializer JsonSerializer { get; private set; }
        }
    }
}
