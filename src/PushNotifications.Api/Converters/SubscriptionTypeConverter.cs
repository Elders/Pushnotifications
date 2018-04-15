using System;
using System.Collections.Generic;
using System.Security.Claims;
using PushNotifications.Contracts.Subscriptions;

namespace PushNotifications.Api.Converters
{
    public class SubscriptionTypeConverter : GenericJsonConverter<string, SubscriptionType>
    {
        public override SubscriptionType Convert(string jObject, Type objectType, IEnumerable<Claim> claims)
        {
            if (ReferenceEquals(jObject, null) == true) return null;
            if (string.IsNullOrEmpty(jObject)) return null;

            return SubscriptionType.Create(jObject);
        }

        public override object GetValue(SubscriptionType instance)
        {
            return instance.ToString();
        }
    }
}
