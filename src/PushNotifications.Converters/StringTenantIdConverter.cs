using System;
using Elders.Cronus;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Security.Claims;
using PushNotifications.Converters.Extensions;

namespace PushNotifications.Converters
{
    public class StringTenantUrnConverter : GenericJsonConverter<string, StringTenantUrn>
    {
        public override bool CanConvertValue(Type valueType, IEnumerable<Claim> claims)
        {
            return typeof(IConvertible).IsAssignableFrom(valueType);
        }

        public override StringTenantUrn Convert(string jObject, Type objectType, IEnumerable<Claim> claims)
        {
            if (string.IsNullOrEmpty(jObject))
                return null;

            if (jObject.CanUrlTokenDecode())
                jObject = jObject.UrlDecode();

            StringTenantUrn x;
            if (StringTenantUrn.TryParse(jObject, out x))
                return x;

            throw new InvalidOperationException($"Unable to create StringTenantUrn based on {jObject}");
        }

        public override object GetValue(StringTenantUrn instance)
        {
            return instance.Value.UrlEncode();
        }
    }

    public class StringTenantIdConverter : GenericJsonConverter<string, StringTenantId>
    {
        public override bool CanConvertValue(Type valueType, IEnumerable<Claim> claims)
        {
            return typeof(IConvertible).IsAssignableFrom(valueType);
        }

        public override StringTenantId Convert(string jObject, Type objectType, IEnumerable<Claim> claims)
        {
            if (string.IsNullOrEmpty(jObject))
                return null;

            if (jObject.CanUrlTokenDecode())
                jObject = jObject.UrlDecode();

            var constructor = objectType.GetConstructors().SingleOrDefault(x => Match(x));
            if (constructor == null)
                throw new InvalidOperationException(string.Format("There is no constructor for '{0}' to match (IUrn urn)", objectType.FullName));

            return (StringTenantId)constructor.Invoke(new object[] { Urn.Parse(jObject) });
        }

        static bool Match(ConstructorInfo info)
        {
            var parameterts = info.GetParameters().ToList().OrderBy(x => x.Position).ToList();
            if (parameterts.Count != 1)
                return false;
            return parameterts[0].Name == "urn" && parameterts[0].ParameterType == (typeof(IUrn));
        }

        public override object GetValue(StringTenantId instance)
        {
            return instance.Urn.Value.UrlEncode();
        }
    }
}
