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
            throw new NotImplementedException($"Unable to resolve tenant based only on type. If you need builder provide {nameof(ProjectionCommit)} to {nameof(MultiTenantProjectionStore)}");
        }

        public IProjectionBuilder GetBuilder(Type projectionType, ProjectionCommit commit)
        {
            var tenant = tenantResolver.Resolve(commit);
            var store = provisioner.GetProjectionStore(tenant);

            return store.GetBuilder(projectionType);
        }

        public ProjectionStream Load(string contractId, IBlobId projectionId, ISnapshot snapshot)
        {
            var tenant = tenantResolver.Resolve(projectionId);
            var store = provisioner.GetProjectionStore(tenant);

            return store.Load(contractId, projectionId, snapshot);
        }

        public void Save(ProjectionCommit commit)
        {
            var tenant = tenantResolver.Resolve(commit);
            var store = provisioner.GetProjectionStore(tenant);

            store.Save(commit);
        }
    }
}