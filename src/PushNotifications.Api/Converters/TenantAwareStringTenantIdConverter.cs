using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using Elders.Cronus.DomainModeling;
using PushNotifications.Converters;
using PushNotifications.Converters.Extensions;

namespace PushNotifications.Api.Converters
{
    public class TenantAwareStringTenantIdConverter : GenericJsonConverter<string, StringTenantId>
    {
        private Dictionary<Type, string> arNames = new Dictionary<Type, string>();

        private string GetArName(Type objectType)
        {
            if (!arNames.ContainsKey(objectType))
            {
                var instance = (StringTenantId)Activator.CreateInstance(objectType, new object[] { "default", "default" });
                var arName = instance.AggregateRootName.ToLowerInvariant();
                if (string.IsNullOrEmpty(arName))
                    throw new NotImplementedException($"Could not get ar name for {objectType}");
                arNames[objectType] = arName;
            }
            return arNames[objectType];
        }

        public override bool CanConvert(Type objectType)
        {
            return base.CanConvert(objectType) && ClaimsPrincipal.Current.Claims.Any(x => x.Type == Elders.Web.Api.AuthorizeClaimType.Tenant || x.Type == Elders.Web.Api.AuthorizeClaimType.TenantClient);
        }

        public override bool CanConvertValue(Type valueType, IEnumerable<Claim> claims)
        {
            return typeof(IConvertible).IsAssignableFrom(valueType) && claims.Any(x => x.Type == Elders.Web.Api.AuthorizeClaimType.Tenant || x.Type == Elders.Web.Api.AuthorizeClaimType.TenantClient);
        }

        public override StringTenantId Convert(string value, Type objectType, IEnumerable<Claim> claims)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            if (value.CanUrlTokenDecode())
                value = value.UrlDecode();

            var tenantClaim = claims.SingleOrDefault(x => x.Type == Elders.Web.Api.AuthorizeClaimType.Tenant || x.Type == Elders.Web.Api.AuthorizeClaimType.TenantClient);
            if (tenantClaim == null)
                return null;

            if (value.StartsWith("urn", StringComparison.OrdinalIgnoreCase))
            {
                var urnParts = value.Split(':');

                if (tenantClaim.Value.ToLowerInvariant() != urnParts[1].ToLowerInvariant())
                    throw new InvalidOperationException($"The urn tenant: {urnParts[1].ToLowerInvariant()} does not match the token tenant:{tenantClaim.Value.ToLowerInvariant()}");

                var expectedArType = GetArName(objectType);
                var urnPrefix = $"urn:{tenantClaim.Value}:";
                if (expectedArType == urnParts[2])
                {
                    urnPrefix = $"urn:{tenantClaim.Value}:{expectedArType}:";
                }

                var stringIdValue = value.Substring(urnPrefix.Length);

                var constructor = objectType.GetConstructors().SingleOrDefault(x => Match(x));
                if (constructor == null)
                    throw new InvalidOperationException(string.Format("There is no constructor for '{0}' to match (string id, string tenant)", objectType.FullName));

                var tenantId = (StringTenantId)constructor.Invoke(new object[] { stringIdValue, tenantClaim.Value });

                return tenantId;
            }
            else
            {
                var constructor = objectType.GetConstructors().SingleOrDefault(x => Match(x));
                if (constructor == null)
                    throw new InvalidOperationException(string.Format("There is no constructor for '{0}' to match (string id, string tenant)", objectType.FullName));

                return (StringTenantId)constructor.Invoke(new object[] { value, tenantClaim.Value });
            }
        }

        private bool Match(ConstructorInfo info)
        {
            var parameterts = info.GetParameters().ToList().OrderBy(x => x.Position).ToList();
            if (parameterts.Count != 2)
                return false;
            return parameterts[0].Name == "id" && parameterts[0].ParameterType == (typeof(string)) &&
                parameterts[1].Name == "tenant" && parameterts[1].ParameterType == (typeof(string));
        }

        public override object GetValue(StringTenantId instance)
        {
            return instance.Urn.Value.UrlEncode();
        }
    }
}
