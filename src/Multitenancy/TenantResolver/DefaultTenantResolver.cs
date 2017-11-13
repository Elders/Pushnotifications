using System;
using System.Text;
using Elders.Cronus.DomainModeling;
using Elders.Cronus.EventStore;

namespace Multitenancy.TenantResolver
{
    public class DefaultTenantResolver : ITenantResolver
    {
        public string Resolve(IAggregateRootId id)
        {
            if (ReferenceEquals(null, id) == true) throw new ArgumentNullException(nameof(id));

            if (id is StringTenantId)
                return ((StringTenantId)id).Tenant;

            throw new NotSupportedException($"Unable to resolve tenant for id {id}");
        }

        public string Resolve(AggregateCommit aggregateCommit)
        {
            if (ReferenceEquals(null, aggregateCommit) == true) throw new ArgumentNullException(nameof(aggregateCommit));

            var urn = Encoding.UTF8.GetString(aggregateCommit.AggregateRootId);
            StringTenantUrn stringTenantUrn;

            if (StringTenantUrn.TryParse(urn, out stringTenantUrn))
                return stringTenantUrn.Tenant;

            throw new NotSupportedException($"Unable to resolve tenant for id {aggregateCommit.AggregateRootId}");
        }
    }
}
