using Elders.Cronus.Pipeline.Config;
using Elders.Cronus.Projections.Cassandra.Config;
using Elders.Cronus.Projections.Cassandra.EventSourcing;
using Elders.Cronus.Projections.Cassandra.Snapshots;
using Elders.Cronus.IocContainer;
using Elders.Cronus.DomainModeling.Projections;
using Multitenancy.TenantResolver;

namespace Multitenancy.Cassandra.Projections
{
    public class MultiTenantCassandraProjectionsStoreSettings : Elders.Cronus.Projections.Cassandra.Config.CassandraProjectionsStoreSettings
    {
        public MultiTenantCassandraProjectionsStoreSettings(ISettingsBuilder settingsBuilder) : base(settingsBuilder)
        { }

        public override void Build()
        {
            var builder = this as ISettingsBuilder;
            ICassandraProjectionsStoreSettings settings = this as ICassandraProjectionsStoreSettings;

            var provisioner = new CassandraProjectionsProvisioner(builder, settings);
            var tenantResolver = new DefaultTenantResolver();

            builder.Container.RegisterSingleton<IProjectionStore>(() => new MultiTenantProjectionStore(provisioner, tenantResolver));
            if (ReferenceEquals(null, settings.ProjectionsToSnapshot))
            {
                builder.Container.RegisterSingleton<ISnapshotStore>(() => new NoSnapshotStore());
            }
            else
            {
                builder.Container.RegisterSingleton<ISnapshotStore>(() => new MultiTenantSnapshotStore(provisioner, tenantResolver));
            }

            builder.Container.RegisterSingleton<ISnapshotStrategy>(() => settings.SnapshotStrategy);
            builder.Container.RegisterTransient<IProjectionRepository>(() => new ProjectionRepository(builder.Container.Resolve<IProjectionStore>(), builder.Container.Resolve<ISnapshotStore>(), builder.Container.Resolve<ISnapshotStrategy>()));
        }
    }

}
