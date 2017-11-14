using Elders.Cronus.Pipeline.Config;
using Elders.Cronus.Projections.Cassandra.Config;
using Elders.Cronus.Projections.Cassandra.EventSourcing;
using Elders.Cronus.Projections.Cassandra.Snapshots;
using Elders.Cronus.IocContainer;
using DataStaxCassandra = Cassandra;
using Elders.Cronus.Serializer;
using Elders.Cronus.DomainModeling.Projections;
using System;
using System.Collections.Generic;
using Multitenancy.TenantResolver;
using Elders.Cronus.DomainModeling;
using Elders.Cronus.Projections;

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
            var tenant = tenantResolver.Resolve(id);
            var store = provisioner.GetSnapshotStore(tenant);

            return store.Load(projectionContractId,id);
        }

        public void Save(ISnapshot snapshot)
        {
            var tenant = tenantResolver.Resolve(snapshot.Id);
            var store = provisioner.GetSnapshotStore(tenant);

             store.Save(snapshot);
        }
    }
}