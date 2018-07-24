using System;
using System.Collections.Generic;
using System.Security.Claims;
using Newtonsoft.Json;
using PushNotifications.Contracts;

namespace PushNotifications.Api.Converters
{
    public class TopicConverter : GenericJsonConverter<string, Topic>
    {
        public override Topic Convert(string jObject, Type objectType, IEnumerable<Claim> claims)
        {
            if (ReferenceEquals(jObject, null) == true) return null;
            if (string.IsNullOrEmpty(jObject)) return null;

            return new Topic(jObject);
        }

        public override object GetValue(Topic instance)
        {
            return instance.ToString();
        }
    }
}
