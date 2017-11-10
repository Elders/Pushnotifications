using System;
using System.Text;
using Elders.Cronus.DomainModeling;
using Elders.Cronus.EventStore;

namespace PushNotifications.WS.Multitenancy
{
    public class DefaultTenantResolver : ITenantResolver
    {
        readonly string defaultTenant;

        public DefaultTenantResolver(string defaultTenant)
        {
            if (string.IsNullOrEmpty(defaultTenant) == true) throw new ArgumentNullException(nameof(defaultTenant));
            this.defaultTenant = defaultTenant;
        }

        public string Resolve(IAggregateRootId id)
        {
            if (ReferenceEquals(null, id) == true) throw new ArgumentNullException(nameof(id));

            if (id is StringTenantId)
                return ((StringTenantId)id).Tenant;

            return defaultTenant;
        }

        public string Resolve(AggregateCommit aggregateCommit)
        {
            if (ReferenceEquals(null, aggregateCommit) == true) throw new ArgumentNullException(nameof(aggregateCommit));

            var urn = Encoding.UTF8.GetString(aggregateCommit.AggregateRootId);
            StringTenantUrn stringTenantUrn;

            if (StringTenantUrn.TryParse(urn, out stringTenantUrn))
                return stringTenantUrn.Tenant;

            return defaultTenant;
        }
    }
}
