using System;

namespace PushNotifications.WS.Multitenancy
{
    public class MultiTenantEventStore : Elders.Cronus.EventStore.IEventStore
    {
        readonly IEventStoreProvisioner provisioner;

        readonly ITenantResolver tenantResolver;

        public MultiTenantEventStore(IEventStoreProvisioner provisioner, ITenantResolver tenantResolver)
        {
            if (ReferenceEquals(null, provisioner) == true) throw new ArgumentNullException(nameof(provisioner));
            if (ReferenceEquals(null, tenantResolver) == true) throw new ArgumentNullException(nameof(tenantResolver));

            this.provisioner = provisioner;
            this.tenantResolver = tenantResolver;
        }

        public void Append(Elders.Cronus.EventStore.AggregateCommit aggregateCommit)
        {
            var tenant = tenantResolver.Resolve(aggregateCommit);
            var store = provisioner.GetEventStore(tenant);
            store.Append(aggregateCommit);
        }

        public Elders.Cronus.EventStore.EventStream Load(Elders.Cronus.DomainModeling.IAggregateRootId aggregateId)
        {
            var tenant = tenantResolver.Resolve(aggregateId);
            var store = provisioner.GetEventStore(tenant);
            return store.Load(aggregateId);
        }
    }
}
