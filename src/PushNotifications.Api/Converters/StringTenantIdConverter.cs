using System;
using Elders.Cronus.DomainModeling;
using System.Linq;
using System.Reflection;
using PushNotifications.Api.Extensions;
using System.Collections.Generic;
using System.Security.Claims;

namespace PushNotifications.Api.Converters
{
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
