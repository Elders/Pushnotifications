using Elders.Cronus.Projections.Cassandra.Snapshots;
using System;
using Multitenancy.TenantResolver;
using Elders.Cronus.DomainModeling;

namespace Multitenancy.Cassandra.Projections
{
    public class MultiTenantSnapshotStore : ISnapshotStore
    {
        readonly ICassandraProjectionsProvisioner provisioner;

        readonly ITenantResolver tenantResolver;

        public MultiTenantSnapshotStore(ICassandraProjectionsProvisioner provisioner, ITenantResolver tenantResolver)
        {
            if (ReferenceEquals(null, provisioner) == true) throw new ArgumentNullException(nameof(provisioner));
            if (ReferenceEquals(null, tenantResolver) == true) throw new ArgumentNullException(nameof(tenantResolver));

            this.provisioner = provisioner;
            this.tenantResolver = tenantResolver;
        }

        public ISnapshot Load(string projectionContractId, IBlobId id)
        {
            if (string.IsNullOrEmpty(projectionContractId) == true) throw new ArgumentNullException(nameof(projectionContractId));
            if (ReferenceEquals(null, id) == true) throw new ArgumentNullException(nameof(id));

            var tenant = tenantResolver.Resolve(id);
            var store = provisioner.GetSnapshotStore(tenant);

            return store.Load(projectionContractId, id);
        }

        public void Save(ISnapshot snapshot)
        {
            if (ReferenceEquals(null, snapshot) == true) throw new ArgumentNullException(nameof(snapshot));

            var tenant = tenantResolver.Resolve(snapshot.Id);
            var store = provisioner.GetSnapshotStore(tenant);

            store.Save(snapshot);
        }
    }
}
