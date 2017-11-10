using Elders.Cronus.IocContainer;

namespace PushNotifications.WS.Multitenancy
{
    public class MultiTenantCassandraEventStoreSettings : Elders.Cronus.Persistence.Cassandra.Config.CassandraEventStoreSettings
    {
        public MultiTenantCassandraEventStoreSettings(Elders.Cronus.Pipeline.Config.ISettingsBuilder settingsBuilder) : base(settingsBuilder) { }

        public override void Build()
        {
            var builder = this as Elders.Cronus.Pipeline.Config.ISettingsBuilder;
            Elders.Cronus.Persistence.Cassandra.Config.ICassandraEventStoreSettings settings = this as Elders.Cronus.Persistence.Cassandra.Config.ICassandraEventStoreSettings;

            var provisioner = new CassandraEventStoreProvisioner(builder, settings);
            var tenantResolver = new DefaultTenantResolver("elders");
            builder.Container.RegisterSingleton<Elders.Cronus.EventStore.IEventStore>(() => new MultiTenantEventStore(provisioner, tenantResolver), builder.Name);
            //builder.Container.RegisterSingleton<IEventStorePlayer>(() => player, builder.Name); probably should not be here
        }
    }
}
