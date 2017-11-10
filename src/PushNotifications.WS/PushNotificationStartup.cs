using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Cassandra;
using Elders.Cronus.AtomicAction.Config;
using Elders.Cronus.AtomicAction.Redis.Config;
using Elders.Cronus.Cluster.Config;
using Elders.Cronus.DomainModeling;
using Elders.Cronus.DomainModeling.Projections;
using Elders.Cronus.EventStore;
using Elders.Cronus.IocContainer;
using Elders.Cronus.Persistence.Cassandra.Config;
using Elders.Cronus.Pipeline.Config;
using Elders.Cronus.Pipeline.Hosts;
using Elders.Cronus.Pipeline.Transport.RabbitMQ.Config;
using Elders.Cronus.Projections.Cassandra.Config;
using Elders.Cronus.Projections.Cassandra.Snapshots;
using Elders.Pandora;
using PushNotifications.Contracts;
using PushNotifications.Delivery.Bulk;
using PushNotifications.Delivery.FireBase;
using PushNotifications.Delivery.Pushy;
using PushNotifications.Ports;
using PushNotifications.Projections;
using PushNotifications.WS.Logging;
using PushNotifications.WS.Multitenancy;
using PushNotifications.WS.Serialization;

namespace PushNotifications.WS
{
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
                    .UseCluster(cluster =>
                        cluster.UseAggregateRootAtomicAction(atomic =>
                        {
                            if (pandora.Get<bool>("enable_redis_atomic_action"))
                                atomic.UseRedis(redis =>
                                    RedisAggregateRootAtomicActionSettingsExtensions.SetConnectionString(redis, pandora.Get("redis_connection_string"))
                                );
                            else
                                atomic.WithInMemory();
                        }))
                    .UsePushNotifications(pandora)
                    .UsePushNotificationProjections(pandora)
                    .UsePorts(pandora)
                    .RegisterBulkFireBaseDelivery(pandora)
                    .RegisterBulkPushyDelivery(pandora)
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
                .UseRabbitMqTransport(x =>
                {
                    x.Server = pandora.Get("rabbitmq_server");
                    x.Port = pandora.Get<int>("rabbitmq_port");
                    x.Username = pandora.Get("rabbitmq_username");
                    x.Password = pandora.Get("rabbitmq_password");
                    x.VirtualHost = pandora.Get("rabbitmq_virtualhost");
                })
                .WithDefaultPublishers()
                .UseMultiTenantCassandraEventStore(eventStore =>
                    CassandraEventStoreExtensions.SetConnectionString(eventStore, pandora.Get("pn_cassandra_event_store_conn_str"))
                    .SetReplicationStrategy(eventStoreReplicationStrategy)
                    .SetWriteConsistencyLevel(pandora.Get<ConsistencyLevel>("pn_cassandra_event_store_write_consistency_level"))
                    .SetReadConsistencyLevel(pandora.Get<ConsistencyLevel>("pn_cassandra_event_store_read_consistency_level"))
                    .SetAggregateStatesAssembly(typeof(PushNotificationsAssembly)))
                .UseApplicationServices(cmdHandler => cmdHandler.RegisterHandlersInAssembly(new[] { typeof(PushNotificationsAssembly).Assembly }, pn_appServiceFactory.Create)));

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
            var cassandraProjetions = typeof(PushNotificationsProjectionsAssembly).Assembly.GetTypes().Where(x => typeof(IProjectionDefinition).IsAssignableFrom(x));

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
                            .UseSnapshots(cassandraProjetions)
                            .UseSnapshotStrategy(new DefaultSnapshotStrategy(TimeSpan.FromDays(10), 500))
                            .SetProjectionTypes(cassandraProjetions)
                            .SetProjectionsReplicationStrategy(projectionsReplicationStrategy)
                            .SetProjectionsWriteConsistencyLevel(pandora.Get<ConsistencyLevel>("pn_cassandra_projections_write_consistency_level"))
                            .SetProjectionsReadConsistencyLevel(pandora.Get<ConsistencyLevel>("pn_cassandra_projections_read_consistency_level")))));

            return cronusSettings;
        }

        static ICronusSettings RegisterBulkFireBaseDelivery(this ICronusSettings cronusSettings, Pandora pandora)
        {
            var fireBaseBaseUrl = "https://fcm.googleapis.com/";
            var serverKey = "AAAAqg7V420:APA91bEggfsid7oJGnlravJ0gwCJ8ZMthEfWTfecHMOQjYVFdToIXxLXQj0oomBeVDNYCFZQ_sfbqASpsGcqOkJdKASpxCxGYHvof3ngENX_iSD_bl65PriDIAESPhhvqNeBZqw0wb4Z";
            var timeSpanBeforeFlush = new TimeSpan(0, 0, 5);
            var recipientsCountBeforeFlush = 100;

            var fireBaseRestClient = new RestSharp.RestClient(fireBaseBaseUrl);
            var fireBasedelivery = new FireBaseDelivery(fireBaseRestClient, NewtonsoftJsonSerializer.Default(), serverKey);
            cronusSettings.Container.RegisterSingleton(() => new BulkDelivery<FireBaseDelivery>(fireBasedelivery, timeSpanBeforeFlush, recipientsCountBeforeFlush));
            return cronusSettings;
        }

        static ICronusSettings RegisterBulkPushyDelivery(this ICronusSettings cronusSettings, Pandora pandora)
        {
            var pushyBaseUrl = "https://api.pushy.me/";
            var serverKey = "cd8100e3f367777f2dd999c5d97c5cc30d099fcf4da073749232140e365e09f9";
            var timeSpanBeforeFlush = new TimeSpan(0, 0, 5);
            var recipientsCountBeforeFlush = 100;

            var pushyRestClient = new RestSharp.RestClient(pushyBaseUrl);
            var pushyDelivery = new PushyDelivery(pushyRestClient, NewtonsoftJsonSerializer.Default(), serverKey);
            cronusSettings.Container.RegisterSingleton(() => new BulkDelivery<PushyDelivery>(pushyDelivery, timeSpanBeforeFlush, recipientsCountBeforeFlush));
            return cronusSettings;
        }


    }

    public class DefaultTenantUdiDahan : ITenantUdiDahan // Strategy? blah
    {
        public string GetTenantFriendlyName(IAggregateRootId id)
        {
            if (id is StringTenantId)
                return ((StringTenantId)id).Tenant;
            else
                return "elders/default";// we could have cross tenant agreegates
                                        //? or throw new NotSupportedException() ?
        }

        public string GetTenantFriendlyName(AggregateCommit aggregateCommit)
        {
            var urn = Encoding.UTF8.GetString(aggregateCommit.AggregateRootId);
            StringTenantUrn stringTenantUrn;

            if (StringTenantUrn.TryParse(urn, out stringTenantUrn))
                return stringTenantUrn.Tenant;

            return "elders/default";// we could have cross tenant agreegates
                                    //? or throw new NotSupportedException() ?
        }
    }
}
