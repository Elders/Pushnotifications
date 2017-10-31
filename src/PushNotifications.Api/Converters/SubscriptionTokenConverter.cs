using System;
using System.Collections.Generic;
using System.Security.Claims;
using PushNotifications.Contracts;

namespace PushNotifications.Api.Converters
{
    public class SubscriptionTokenConverter : GenericJsonConverter<string, SubscriptionToken>
    {
        public override SubscriptionToken Convert(string jObject, Type objectType, IEnumerable<Claim> claims)
        {
            if (ReferenceEquals(jObject, null) == true) return null;
            if (string.IsNullOrEmpty(jObject)) return null;

            return new SubscriptionToken(jObject);
        }

        public override object GetValue(SubscriptionToken instance)
        {
            return instance;
        }
    }
}
