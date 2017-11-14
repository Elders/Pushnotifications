using System;
using System.Text;
using Elders.Cronus.DomainModeling;
using Elders.Cronus.EventStore;
using Elders.Cronus.Projections;

namespace Multitenancy.TenantResolver
{
    public class DefaultTenantResolver : ITenantResolver
    {
        public string Resolve(ProjectionCommit projectionCommit)
        {
            var tenant = string.Empty;
            if (TryResolve(projectionCommit.ProjectionId.RawId, out tenant))
                return tenant;

            throw new NotSupportedException($"Unable to resolve tenant for id {projectionCommit.ProjectionId}");
        }

        public string Resolve(IBlobId id)
        {
            var tenant = string.Empty;
            if (TryResolve(id.RawId, out tenant))
                return tenant;

            throw new NotSupportedException($"Unable to resolve tenant for id {id}");
        }

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

            var tenant = string.Empty;
            if (TryResolve(aggregateCommit.AggregateRootId, out tenant))
                return tenant;

            throw new NotSupportedException($"Unable to resolve tenant for id {aggregateCommit.AggregateRootId}");
        }

        bool TryResolve(byte[] id, out string tenant)
        {
            tenant = string.Empty;
            var urn = Encoding.UTF8.GetString(id);
            StringTenantUrn stringTenantUrn;

            if (StringTenantUrn.TryParse(urn, out stringTenantUrn))
            {
                tenant = stringTenantUrn.Tenant;
                return true;
            }

            return false;
        }
    }
}
