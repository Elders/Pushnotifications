using Elders.Cronus.Pipeline.Config;
using Elders.Cronus.Projections.Cassandra.Config;
using Elders.Cronus.Projections.Cassandra.EventSourcing;
using Elders.Cronus.Projections.Cassandra.Snapshots;
using Elders.Cronus.IocContainer;
using System;

namespace Multitenancy.Cassandra.Projections
{
    public class MultiTenantCassandraProjectionsSettings : MultiTenantCassandraProjectionsStoreSettings
    {
        private ISubscrptionMiddlewareSettings subscrptionMiddlewareSettings;

        public MultiTenantCassandraProjectionsSettings(ISettingsBuilder settingsBuilder, ISubscrptionMiddlewareSettings subscrptionMiddlewareSettings) : base(settingsBuilder)
        {
            if (ReferenceEquals(null, subscrptionMiddlewareSettings) == true) throw new ArgumentNullException(nameof(subscrptionMiddlewareSettings));
            this.subscrptionMiddlewareSettings = subscrptionMiddlewareSettings;
        }

        public override void Build()
        {
            var builder = this as ISettingsBuilder;
            ICassandraProjectionsStoreSettings settings = this as ICassandraProjectionsStoreSettings;
            base.Build();
            subscrptionMiddlewareSettings.Middleware(x => { return new EventSourcedProjectionsMiddleware(builder.Container.Resolve<IProjectionStore>(), builder.Container.Resolve<ISnapshotStore>(), builder.Container.Resolve<ISnapshotStrategy>()); });
        }
    }
}
