using System;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PushNotifications.Api.Attributes;

namespace PushNotifications.Api.Converters
{
    public class ClaimFillConverter : JsonConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var instance = Activator.CreateInstance(objectType);
            var jObject = JObject.Load(reader);

            var props = objectType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var properties = props.Where(x => x.CustomAttributes.Any(y => y.AttributeType == typeof(ClaimsIdentityAttribute)));

            foreach (var item in jObject.Properties())
            {
                var property = props.Where(x => x.Name.ToLowerInvariant() == item.Name.ToLowerInvariant()).SingleOrDefault();
                if (property != null)
                {
                    var value = item.Value;
                    var propinstance = serializer.Deserialize(value.CreateReader(), property.PropertyType);
                    if (propinstance != null)
                        property.SetValue(instance, propinstance);
                }
            }

            foreach (var item in properties)
            {
                if (item.GetValue(instance) != GetDefault(objectType))
                    continue;

                var attribute = item.GetCustomAttributes().Single(y => y.GetType() == typeof(ClaimsIdentityAttribute));
                var claimsAttribute = (attribute as ClaimsIdentityAttribute);
                var claim = ClaimsPrincipal.Current.Claims.FirstOrDefault(x => claimsAttribute.ClaimTypes.Contains(x.Type));
                if (ReferenceEquals(null, claim) == false)
                {
                    var token = JToken.FromObject(claim.Value);
                    item.SetValue(instance, serializer.Deserialize(token.CreateReader(), item.PropertyType));
                }

            }
            return instance;
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override bool CanConvert(Type objectType)
        {
            var props = objectType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            return props.Any(x => x.CustomAttributes.Any(y => y.AttributeType == typeof(ClaimsIdentityAttribute)));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        private object GetDefault(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }
    }
}
