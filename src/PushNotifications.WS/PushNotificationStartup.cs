using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cassandra;
using Elders.Cronus;
using Elders.Cronus.AtomicAction.Config;
using Elders.Cronus.AtomicAction.Redis.Config;
using Elders.Cronus.Cluster.Config;
using Elders.Cronus.FaultHandling;
using Elders.Cronus.FaultHandling.Strategies;
using Elders.Cronus.IocContainer;
using Elders.Cronus.MessageProcessing;
using Elders.Cronus.Middleware;
using Elders.Cronus.Multitenancy;
using Elders.Cronus.Persistence.Cassandra.Config;
using Elders.Cronus.Pipeline.Config;
using Elders.Cronus.Pipeline.Hosts;
using Elders.Cronus.Pipeline.Transport.RabbitMQ.Config;
using Elders.Cronus.Projections;
using Elders.Cronus.Projections.Cassandra.Config;
using Elders.Cronus.Projections.Versioning;
using Elders.Pandora;
using Multitenancy.Delivery;
using PushNotifications.Contracts;
using PushNotifications.Ports;
using PushNotifications.Projections;
using PushNotifications.WS.Logging;

namespace PushNotifications.WS
{
    public class PushNotificationTenants : ITenantList
    {
        private readonly IEnumerable<string> tenants;

        public PushNotificationTenants(IEnumerable<string> tenants)
        {
            this.tenants = tenants;
        }

        public IEnumerable<string> GetTenants() => tenants;
    }

    public static class PushNotificationStartUp
    {
        static CronusHost host;
        static ILog log;
        static Container containerWhichYouShouldNotUse;

        public static void Start(Pandora pandora)
        {
            try
            {
                log = LogProvider.GetLogger(typeof(PushNotificationStartUp));
                log.Info("STARTING => PushNotification Windows Service Host");

                containerWhichYouShouldNotUse = new Container();
                new CronusSettings(containerWhichYouShouldNotUse)
                    .WithTenants(new PushNotificationTenants(pandora.Get<List<string>>("tenants")))
                    .UseCluster(cluster =>
                        cluster.UseAggregateRootAtomicAction(atomic =>
                        {
                            if (pandora.Get<bool>("enable_redis_atomic_action"))
                                atomic.UseRedis(redis =>
                                    RedisAggregateRootAtomicActionSettingsExtensions.SetConnectionString(redis, pandora.Get("redis_endpoints"))
                                );
                            else
                                atomic.WithInMemory();
                        }))
                    .UsePushNotifications(pandora)
                    .UsePushNotificationProjections(pandora)
                    .UsePorts(pandora)
                    .UseMultiTenantDelivery(pandora)
                    .Build();

                host = containerWhichYouShouldNotUse.Resolve<CronusHost>();
                host.Start();

                log.Info("STARTED => PushNotification Windows Service Host");
            }
            catch (Exception ex)
            {
                log.ErrorException(ex.Message, ex);
                throw;
            }
        }

        public static void Stop()
        {
            try
            {
                host.Stop();
                host = null;
                containerWhichYouShouldNotUse.Destroy();
            }
            catch (Exception ex)
            {
                log.FatalException("Unable to stop Cronus properly. Exiting without crashing. There is a chance that some resources are not disposed properly. See container.Destroy()", ex);
            }
        }

        static ICronusSettings UsePushNotifications(this ICronusSettings cronusSettings, Pandora pandora)
        {
            string pnInstanceName = "pn_services";
            var pn_appServiceFactory = new ApplicationServiceFactory(containerWhichYouShouldNotUse, pnInstanceName);

            var eventStoreReplicationFactor = pandora.Get<int>("pn_cassandra_event_store_replication_factor");
            Elders.Cronus.Persistence.Cassandra.ReplicationStrategies.ICassandraReplicationStrategy eventStoreReplicationStrategy = new Elders.Cronus.Persistence.Cassandra.ReplicationStrategies.SimpleReplicationStrategy(eventStoreReplicationFactor);
            if (pandora.Get("pn_cassandra_event_store_replication_strategy") == "network_topology")
            {
                var settings = new List<Elders.Cronus.Persistence.Cassandra.ReplicationStrategies.NetworkTopologyReplicationStrategy.DataCenterSettings>();
                foreach (var datacenter in pandora.Get<List<string>>("pn_cassandra_event_store_data_centers"))
                {
                    var setting = new Elders.Cronus.Persistence.Cassandra.ReplicationStrategies.NetworkTopologyReplicationStrategy.DataCenterSettings(datacenter, eventStoreReplicationFactor);
                    settings.Add(setting);
                }
                eventStoreReplicationStrategy = new Elders.Cronus.Persistence.Cassandra.ReplicationStrategies.NetworkTopologyReplicationStrategy(settings);
            }

            cronusSettings
                .UseContractsFromAssemblies(new[]
                {
                     Assembly.GetAssembly(typeof(PushNotificationsContractsAssembly)),
                     Assembly.GetAssembly(typeof(PushNotificationsAssembly)),
                     Assembly.GetAssembly(typeof(PushNotificationsProjectionsAssembly))
                })
                .UseCommandConsumer(pnInstanceName, consumer => consumer
                    .SetNumberOfConsumerThreads(5)
                    .UseRabbitMqTransport(x =>
                    {
                        x.Server = pandora.Get("rabbitmq_server");
                        x.Port = pandora.Get<int>("rabbitmq_port");
                        x.Username = pandora.Get("rabbitmq_username");
                        x.Password = pandora.Get("rabbitmq_password");
                        x.VirtualHost = pandora.Get("rabbitmq_virtualhost");
                    })
                    .WithDefaultPublishers()
                    .UseCassandraEventStore(eventStore =>
                        CassandraEventStoreExtensions.SetConnectionString(eventStore, pandora.Get("pn_cassandra_event_store_conn_str"))
                        .SetReplicationStrategy(eventStoreReplicationStrategy)
                        .SetWriteConsistencyLevel(pandora.Get<ConsistencyLevel>("pn_cassandra_event_store_write_consistency_level"))
                        .SetReadConsistencyLevel(pandora.Get<ConsistencyLevel>("pn_cassandra_event_store_read_consistency_level"))
                        .SetBoundedContext(typeof(PushNotificationsAssembly).Assembly.GetBoundedContext().BoundedContextName))
                    .UseApplicationServices(cmdHandler => cmdHandler
                        .RegisterHandlersInAssembly(new[] { typeof(PushNotificationsAssembly).Assembly }, pn_appServiceFactory.Create).Middleware(x => new InMemoryRetryMiddlewareCustom<HandleContext>(x))));

            return cronusSettings;
        }

        static ICronusSettings UsePorts(this ICronusSettings cronusSettings, Pandora pandora)
        {
            var pnPortsName = "pn_ports";
            var pnProjHandlerFactory = new ServiceLocator(cronusSettings.Container);
            var ports = typeof(PushNotificationsPortsAssembly).Assembly.GetTypes().Where(x => typeof(IPort).IsAssignableFrom(x));

            cronusSettings
               .UsePortConsumer(pnPortsName, x => x
                   .WithDefaultPublishers()
                   .UseRabbitMqTransport(mq =>
                   {
                       mq.Server = pandora.Get("rabbitmq_server");
                       mq.Port = pandora.Get<int>("rabbitmq_port");
                       mq.Username = pandora.Get("rabbitmq_username");
                       mq.Password = pandora.Get("rabbitmq_password");
                       mq.VirtualHost = pandora.Get("rabbitmq_virtualhost");
                   })
                   .UsePorts(p => p.RegisterHandlerTypes(ports, pnProjHandlerFactory.Resolve)));

            return cronusSettings;
        }

        static ICronusSettings UsePushNotificationProjections(this ICronusSettings cronusSettings, Pandora pandora)
        {
            var pnEventSourcedProjectionsName = "pn_event_sourced_projections";
            var pnProjHandlerFactory = new ServiceLocator(cronusSettings.Container);
            var systemProjections = typeof(PersistentProjectionVersionHandler).Assembly.GetTypes().Where(x => typeof(IProjectionDefinition).IsAssignableFrom(x)).ToList();
            var cassandraProjetions = typeof(PushNotificationsProjectionsAssembly).Assembly.GetTypes().Where(x => typeof(IProjectionDefinition).IsAssignableFrom(x)).ToList();
            cassandraProjetions.AddRange(systemProjections);

            // Cassandra persistent projections
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

            cronusSettings
                .UseProjectionConsumer(pnEventSourcedProjectionsName, consumable => consumable
                    .WithDefaultPublishers()
                    .UseRabbitMqTransport(x =>
                    {
                        x.Server = pandora.Get("rabbitmq_server");
                        x.Port = pandora.Get<int>("rabbitmq_port");
                        x.Username = pandora.Get("rabbitmq_username");
                        x.Password = pandora.Get("rabbitmq_password");
                        x.VirtualHost = pandora.Get("rabbitmq_virtualhost");
                    })
                     .UseProjections(h => h
                        .RegisterHandlerTypes(cassandraProjetions, pnProjHandlerFactory.Resolve)
                        .UseCassandraProjections(p => p
                            .SetProjectionsConnectionString(pandora.Get("pn_cassandra_projections"))
                            .SetProjectionTypes(cassandraProjetions)
                            .SetProjectionsReplicationStrategy(projectionsReplicationStrategy)
                            .SetProjectionsWriteConsistencyLevel(pandora.Get<ConsistencyLevel>("pn_cassandra_projections_write_consistency_level"))
                            .SetProjectionsReadConsistencyLevel(pandora.Get<ConsistencyLevel>("pn_cassandra_projections_read_consistency_level"))))
                    .UseProjectionsIndex(index => index.RegisterHandlersInAssembly(new[] { typeof(PushNotificationsProjectionsAssembly).Assembly })));

            return cronusSettings;
        }

        public static ICronusSettings UseCronusSystemServices(this ICronusSettings cronusSettings, Pandora pandora)
        {
            var projectionsReplicationFactor = pandora.Get<int>("iaa_cassandra_projections_replication_factor");
            Elders.Cronus.Projections.Cassandra.ReplicationStrategies.ICassandraReplicationStrategy projectionsReplicationStrategy = new Elders.Cronus.Projections.Cassandra.ReplicationStrategies.SimpleReplicationStrategy(projectionsReplicationFactor);
            if (pandora.Get("iaa_cassandra_projections_replication_strategy") == "network_topology")
            {
                var settings = new List<Elders.Cronus.Projections.Cassandra.ReplicationStrategies.NetworkTopologyReplicationStrategy.DataCenterSettings>();
                foreach (var datacenter in pandora.Get<List<string>>("iaa_cassandra_projections_data_centers"))
                {
                    var setting = new Elders.Cronus.Projections.Cassandra.ReplicationStrategies.NetworkTopologyReplicationStrategy.DataCenterSettings(datacenter, projectionsReplicationFactor);
                    settings.Add(setting);
                }
                projectionsReplicationStrategy = new Elders.Cronus.Projections.Cassandra.ReplicationStrategies.NetworkTopologyReplicationStrategy(settings);
            }

            var eventStoreReplicationFactor = pandora.Get<int>("pn_cassandra_event_store_replication_factor");
            Elders.Cronus.Persistence.Cassandra.ReplicationStrategies.ICassandraReplicationStrategy eventStoreReplicationStrategy = new Elders.Cronus.Persistence.Cassandra.ReplicationStrategies.SimpleReplicationStrategy(eventStoreReplicationFactor);
            if (pandora.Get("pn_cassandra_event_store_replication_strategy") == "network_topology")
            {
                var settings = new List<Elders.Cronus.Persistence.Cassandra.ReplicationStrategies.NetworkTopologyReplicationStrategy.DataCenterSettings>();
                foreach (var datacenter in pandora.Get<List<string>>("pn_cassandra_event_store_data_centers"))
                {
                    var setting = new Elders.Cronus.Persistence.Cassandra.ReplicationStrategies.NetworkTopologyReplicationStrategy.DataCenterSettings(datacenter, eventStoreReplicationFactor);
                    settings.Add(setting);
                }
                eventStoreReplicationStrategy = new Elders.Cronus.Persistence.Cassandra.ReplicationStrategies.NetworkTopologyReplicationStrategy(settings);
            }

            var systemSaga_serviceLocator = new ServiceLocator(cronusSettings.Container, "SystemSaga");
            cronusSettings.UseSagaConsumer("SystemSaga", consumer => consumer
                .SetNumberOfConsumerThreads(5)
                .WithDefaultPublishers()
                .UseRabbitMqTransport(x =>
                {
                    x.Server = pandora.Get("rabbitmq_server");
                    x.Port = pandora.Get<int>("rabbitmq_port");
                    x.Username = pandora.Get("rabbitmq_username");
                    x.Password = pandora.Get("rabbitmq_password");
                    x.VirtualHost = pandora.Get("rabbitmq_virtualhost");
                })
                .ConfigureCassandraProjectionsStore(proj => proj
                    .SetProjectionsConnectionString(pandora.Get("pn_cassandra_projections"))
                    .SetProjectionsReplicationStrategy(projectionsReplicationStrategy)
                    .SetProjectionsWriteConsistencyLevel(pandora.Get<ConsistencyLevel>("pn_cassandra_projections_write_consistency_level"))
                    .SetProjectionsReadConsistencyLevel(pandora.Get<ConsistencyLevel>("pn_cassandra_projections_read_consistency_level"))
                    .SetProjectionTypes(typeof(ProjectionBuilder).Assembly)
                    .SetProjectionsRetryPolicy(new DefaultRetryPolicy())
                    .SetProjectionsReconnectionPolicy(new ExponentialReconnectionPolicy(100, 100000))
                    .SetProjectionsConnectionString(pandora.Get("pn_cassandra_projections")))
                .UseCassandraEventStore(eventStore =>
                    CassandraEventStoreExtensions.SetConnectionString(eventStore, pandora.Get("pn_cassandra_event_store_conn_str"))
                    .SetReplicationStrategy(eventStoreReplicationStrategy)
                    .SetWriteConsistencyLevel(pandora.Get<ConsistencyLevel>("pn_cassandra_event_store_write_consistency_level"))
                    .SetReadConsistencyLevel(pandora.Get<ConsistencyLevel>("pn_cassandra_event_store_read_consistency_level"))
                    .SetBoundedContext(typeof(PushNotificationsAssembly).Assembly.GetBoundedContext().BoundedContextName))
                .UseSystemSagas(saga => saga.RegisterHandlerTypes(new List<Type>() { typeof(ProjectionBuilder) }, systemSaga_serviceLocator.Resolve))
                );


            return cronusSettings;
        }

        static ICronusSettings UseMultiTenantDelivery(this ICronusSettings cronusSettings, Pandora pandora)
        {
            cronusSettings.Container.RegisterSingleton<IDeliveryProvisioner>(() => new PandoraMultiTenantDeliveryProvisioner(pandora));
            return cronusSettings;
        }
    }

    public class InMemoryRetryMiddlewareCustom<TContext> : Middleware<TContext>
    {
        static ILog log;

        private Elders.Cronus.FaultHandling.RetryPolicy retryPolicy;

        readonly Middleware<TContext> middleware;

        public InMemoryRetryMiddlewareCustom(Middleware<TContext> middleware)
        {
            log = LogProvider.GetLogger(typeof(WS.InMemoryRetryMiddlewareCustom<TContext>));
            this.middleware = middleware;
            var retryStrategy = new Incremental(4, TimeSpan.FromMilliseconds(300), TimeSpan.FromMilliseconds(150));//Total 4 Retrues
            retryPolicy = new Elders.Cronus.FaultHandling.RetryPolicy(new TransientErrorCatchAllStrategy(), retryStrategy);
        }
        protected override void Run(Execution<TContext> execution)
        {
            var guid = Guid.NewGuid();
            retryPolicy.ExecuteAction(() =>
            {
                try
                {
                    middleware.Run(execution.Context);
                }
                catch (Exception ex)
                {
                    log.WarnException($"InMemoryRetryMiddlewareCustom: Error with guid {guid}. Exception message: {ex?.Message}. Inner exception: {ex?.InnerException?.Message}", ex);
                    throw;
                }
            });
        }
    }
}
