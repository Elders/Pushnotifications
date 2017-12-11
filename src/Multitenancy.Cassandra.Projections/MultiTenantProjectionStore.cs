using Elders.Cronus.Projections.Cassandra.EventSourcing;
using Elders.Cronus.Projections.Cassandra.Snapshots;
using System;
using Multitenancy.TenantResolver;
using Elders.Cronus.DomainModeling;
using Elders.Cronus.Projections;

namespace Multitenancy.Cassandra.Projections
{
    public class MultiTenantProjectionStore : IProjectionStore
    {
        readonly ICassandraProjectionsProvisioner provisioner;

        readonly ITenantResolver tenantResolver;

        public MultiTenantProjectionStore(ICassandraProjectionsProvisioner provisioner, ITenantResolver tenantResolver)
        {
            if (ReferenceEquals(null, provisioner) == true) throw new ArgumentNullException(nameof(provisioner));
            if (ReferenceEquals(null, tenantResolver) == true) throw new ArgumentNullException(nameof(tenantResolver));

            this.provisioner = provisioner;
            this.tenantResolver = tenantResolver;
        }

        public IProjectionBuilder GetBuilder(Type projectionType)
        {
            if (ReferenceEquals(null, projectionType) == true) throw new ArgumentNullException(nameof(projectionType));
            throw new NotImplementedException($"Unable to resolve tenant based only on type. If you need builder provide {nameof(ProjectionCommit)} to {nameof(MultiTenantProjectionStore)}");
        }

        public IProjectionBuilder GetBuilder(Type projectionType, ProjectionCommit commit)
        {
            if (ReferenceEquals(null, projectionType) == true) throw new ArgumentNullException(nameof(projectionType));
            if (ReferenceEquals(null, commit) == true) throw new ArgumentNullException(nameof(commit));

            var tenant = tenantResolver.Resolve(commit);
            var store = provisioner.GetProjectionStore(tenant);

            return store.GetBuilder(projectionType);
        }

        public ProjectionStream Load(string contractId, IBlobId projectionId, ISnapshot snapshot)
        {
            if (string.IsNullOrEmpty(contractId) == true) throw new ArgumentNullException(nameof(contractId));
            if (ReferenceEquals(null, projectionId) == true) throw new ArgumentNullException(nameof(projectionId));
            if (ReferenceEquals(null, snapshot) == true) throw new ArgumentNullException(nameof(snapshot));

            var tenant = tenantResolver.Resolve(projectionId);
            var store = provisioner.GetProjectionStore(tenant);

            return store.Load(contractId, projectionId, snapshot);
        }

        public void Save(ProjectionCommit commit)
        {
            if (ReferenceEquals(null, commit) == true) throw new ArgumentNullException(nameof(commit));

            var tenant = tenantResolver.Resolve(commit);
            var store = provisioner.GetProjectionStore(tenant);

            store.Save(commit);
        }
    }
}
