using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Http;
using Cassandra;
using Consul;
using Discovery.Consul;
using Discovery.Contracts;
using Elders.Cronus.AtomicAction.Config;
using Elders.Cronus.AtomicAction.Redis.Config;
using Elders.Cronus.Cluster.Config;
using Elders.Cronus;
using Elders.Cronus.IocContainer;
using Elders.Cronus.Pipeline;
using Elders.Cronus.Pipeline.Config;
using Elders.Cronus.Pipeline.Hosts;
using Elders.Cronus.Pipeline.Transport;
using Elders.Cronus.Pipeline.Transport.RabbitMQ.Config;
using Elders.Cronus.Projections.Cassandra.Config;
using Elders.Cronus.Projections.Cassandra.Snapshots;
using Elders.Cronus.Serializer;
using Elders.Pandora;
using PushNotifications.Api.Host.Logging;
using PushNotifications.Contracts;
using PushNotifications.Projections;
using Elders.Cronus.Projections.Snapshotting;
using Elders.Cronus.Projections.Versioning;
using Elders.Cronus.Projections;

namespace PushNotifications.Api.Host.App_Start
{
    public static class CronusConfig
    {
        static ILog log;

        static IContainer container;

        public static void Register(HttpConfiguration config, Pandora pandora)
        {
            try
            {
                log = LogProvider.GetLogger(typeof(CronusConfig));
                log.Info("Starting PushNotifications API");
                container = new Container();

                var serviceLocator = new ServiceLocator(container);

                var cfg = new CronusSettings(container)
                    .UseCluster(cluster =>
                         cluster.UseAggregateRootAtomicAction(atomic =>
                         {
                             if (pandora.Get<bool>("enable_redis_atomic_action"))
                                 atomic.UseRedis(redis => redis
                                     .SetConnectionString(pandora.Get("redis_endpoints"))
                                 );
                             else
                                 atomic.WithInMemory();
                         })
                     )
                 .UseContractsFromAssemblies(new[]
                 {
                     Assembly.GetAssembly(typeof(PushNotificationsContractsAssembly)),
                     Assembly.GetAssembly(typeof(PushNotificationsApiAssembly)),
                     Assembly.GetAssembly(typeof(PushNotificationsProjectionsAssembly))
                 })
                 .UseRabbitMqTransport(x =>
                 {
                     x.Server = pandora.Get("rabbitmq_server");
                     x.Port = pandora.Get<int>("rabbitmq_port");
                     x.Username = pandora.Get("rabbitmq_username");
                     x.Password = pandora.Get("rabbitmq_password");
                     x.VirtualHost = pandora.Get("rabbitmq_virtualhost");
                 })
                .ConfigureCassandraProjectionsStore(x => x
                    .SetProjectionsConnectionString(pandora.Get("pn_cassandra_projections"))
                    .SetProjectionsReplicationStrategy(GetProjectionsReplicationStrategy(pandora))
                    .SetProjectionsWriteConsistencyLevel(pandora.Get<ConsistencyLevel>("pn_cassandra_projections_write_consistency_level"))
                    .SetProjectionsReadConsistencyLevel(pandora.Get<ConsistencyLevel>("pn_cassandra_projections_read_consistency_level"))
                    .SetProjectionTypes(Assembly.GetAssembly(typeof(PushNotificationsProjectionsAssembly))));
                (cfg as ISettingsBuilder).Build();

                //Func<ITransport> transport = () => container.Resolve<ITransport>();
                //Func<ISerializer> serializer = () => container.Resolve<ISerializer>();
                //container.RegisterSingleton<IPublisher<ICommand>>(() => transport().GetPublisher<ICommand>(serializer()));
                container.RegisterSingleton<InMemoryProjectionVersionStore>(() => new InMemoryProjectionVersionStore());
                container.RegisterSingleton<IProjectionLoader>(() => new ProjectionRepository(container.Resolve<IProjectionStore>(), container.Resolve<ISnapshotStore>(), container.Resolve<ISnapshotStrategy>(), container.Resolve<InMemoryProjectionVersionStore>()));

                container.RegisterSingleton<ConsulClient>(() => new ConsulClient(x => x.Address = ConsulHelper.DefaultConsulUri));
                container.RegisterSingleton<IDiscoveryReader>(() => new ConsulDiscoveryReader(container.Resolve<ConsulClient>()));

                config.Services.Replace(typeof(System.Web.Http.Dispatcher.IHttpControllerActivator), new ServiceLocatorFactory(new ServiceLocator(container)));
            }
            catch (Exception ex)
            {
                log.ErrorException(ex.Message, ex);
                throw;
            }
        }

        static Elders.Cronus.Projections.Cassandra.ReplicationStrategies.ICassandraReplicationStrategy GetProjectionsReplicationStrategy(Pandora pandora)
        {
            var projectionsReplicationFactor = pandora.Get<int>("pn_cassandra_projections_replication_factor");
            Elders.Cronus.Projections.Cassandra.ReplicationStrategies.ICassandraReplicationStrategy projectionsReplicationStrategy = new Elders.Cronus.Projections.Cassandra.ReplicationStrategies.SimpleReplicationStrategy(projectionsReplicationFactor);
            if (pandora.Get("pn_cassandra_projections_replication_strategy") == "network_topology")
            {
                var settings = new List<Elders.Cronus.Projections.Cassandra.ReplicationStrategies.NetworkTopologyReplicationStrategy.DataCenterSettings>();
                foreach (var datacenter in pandora.Get<List<string>>("pn_cassandra_projections_data_centers"))
                {
                    var setting = new Elders.Cronus.Projections.Cassandra.ReplicationStrategies.NetworkTopologyReplicationStrategy.DataCenterSettings(datacenter, projectionsReplicationFactor);
                    settings.Add(setting);
                }

                projectionsReplicationStrategy = new Elders.Cronus.Projections.Cassandra.ReplicationStrategies.NetworkTopologyReplicationStrategy(settings);
            }


            return projectionsReplicationStrategy;
        }
    }
}
