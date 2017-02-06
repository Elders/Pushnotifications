using System;
using System.IO;
using System.Reflection;
using Elders.Cronus.DomainModeling.Projections;
using Elders.Cronus.IocContainer;
using Elders.Cronus.Persistence.Cassandra.Config;
using Elders.Cronus.Pipeline.Config;
using Elders.Cronus.Pipeline.Hosts;
using Elders.Cronus.Pipeline.Transport.RabbitMQ.Config;
using Elders.Cronus.Projections.Cassandra;
using Elders.Cronus.Serializer;
using Elders.Pandora;
using PushNotifications.Contracts.PushNotifications.Events;
using PushNotifications.Ports.APNS;
using PushNotifications.PushNotifications;
using PushNotifications.WS.Logging;
using PushSharp;
using PushSharp.Android;
using PushSharp.Apple;
using PushSharp.Core;
using Elders.Cronus.Cluster.Config;
using Elders.Cronus.AtomicAction.Config;
using Elders.Cronus.AtomicAction.Redis.Config;
using Projections;
using Projections.Cassandra;
using Facilities.Factory;
using PushNotifications.Throttling;
using PushNotifications.APNS;
using PushNotifications.GCM;
using PushNotifications.Pushy;

namespace PushNotifications.WS
{
    public static class Cronus
    {
        static ILog log = LogProvider.GetLogger(typeof(Cronus));

        static CronusHost host;
        static Container container;

        public static void Start()
        {
            try
            {
                var appContext = new ApplicationContext("PushNotifications");
                var cfgRepo = new ConsulForPandora(new Uri("http://consul.local.com:8500"));
                var pandora = new Pandora(appContext, cfgRepo);

                LogStartup.Boot(pandora);

                log.Info("Starting Cronus Push Notifications");

                container = new Container();

                new CronusSettings(container)
                     .UseCluster(cluster =>
                        cluster.UseAggregateRootAtomicAction(atomic =>
                        {
                            if (pandora.Get<bool>("enable_redis_atomic_action"))
                                atomic.UseRedis(redis => redis.SetLockEndPoints(pandora.Get("redis_endpoints").ParseIPEndPoints(";")));
                            else
                                atomic.WithInMemory();
                        }))
                     .UseAppServices(pandora)
                     .UseProjections(pandora)
                     .UsePorts(pandora)
                     .Build();

                host = container.Resolve<CronusHost>();
                host.Start();
                log.Info("STARTED Cronus Push Notifications");
            }
            catch (Exception ex)
            {
                log.ErrorException("Unable to boot PushNotifications.WS", ex);
                throw;
            }
        }

        private static ICronusSettings UseAppServices(this ICronusSettings cronusSettings, Pandora pandora)
        {
            var appServiceFactory = new ServiceLocator(cronusSettings.Container);

            cronusSettings.UseContractsFromAssemblies(new[] {
                    Assembly.GetAssembly(typeof(PushNotificationWasSent)),
                    Assembly.GetAssembly(typeof(APNSSubscriptionsProjection)),
                    Assembly.GetAssembly(typeof(APNSNotificationMessage)),
                    Assembly.GetAssembly(typeof(GCMNotificationMessage)),
                    Assembly.GetAssembly(typeof(PushyNotificationMessage))
                })
            .UseCommandConsumer(consumer => consumer
                .UseRabbitMqTransport(x =>
                {
                    x.Server = pandora.Get("rabbitmq_server");
                    x.Port = pandora.Get<int>("rabbitmq_port");
                    x.AdminPort = pandora.Get<int>("rabbitmq_admin_port");
                    x.Username = pandora.Get("rabbitmq_username");
                    x.Password = pandora.Get("rabbitmq_password");
                    x.VirtualHost = pandora.Get("rabbitmq_virtualhost");
                })
                .WithDefaultPublishers()
                .UseCassandraEventStore(eventStore => eventStore
                    .SetConnectionString(pandora.Get("pushnot_conn_str_es"))
                    .SetAggregateStatesAssembly(typeof(PushNotificationState))
                    .WithNewStorageIfNotExists())
                .UseApplicationServices(cmdHandler => cmdHandler.RegisterHandlersInAssembly(new[] { typeof(PushNotificationAppService).Assembly }, appServiceFactory.Resolve)));

            return cronusSettings;
        }

        private static ICronusSettings UseProjections(this ICronusSettings cronusSettings, Pandora pandora)
        {
            var projectionFactory = new ServiceLocator(cronusSettings.Container);

            var projectionSession = SessionFactory.Create(pandora.Get("pushnot_conn_str_projections"));
            projectionSession.InitializeProjectionDatabase(Assembly.GetAssembly(typeof(APNSSubscriptionsProjection)));
            var persister = new CassandraPersister(projectionSession);
            Func<ISerializer> serializer = () => container.Resolve<ISerializer>();
            Func<Repository> repo = () => new Repository(persister, obj => serializer().SerializeToBytes(obj), data => serializer().DeserializeFromBytes(data));
            container.RegisterTransient<IProjectionRepository>(() => new CassandraProjectionRepository(new ProjectionBuilder(), repo));
            container.RegisterTransient<IRepository>(() => new Repository(
                new CassandraPersister(container.Resolve<Cassandra.ISession>()),
                container.Resolve<ISerializer>().SerializeToBytes,
                container.Resolve<ISerializer>().DeserializeFromBytes));

            cronusSettings.UseProjectionConsumer(consumable => consumable
                .WithDefaultPublishers()
                .UseRabbitMqTransport(x =>
                {
                    x.Server = pandora.Get("rabbitmq_server");
                    x.Port = pandora.Get<int>("rabbitmq_port");
                    x.AdminPort = pandora.Get<int>("rabbitmq_admin_port");
                    x.Username = pandora.Get("rabbitmq_username");
                    x.Password = pandora.Get("rabbitmq_password");
                    x.VirtualHost = pandora.Get("rabbitmq_virtualhost");
                })
                .UseProjections(h => h.RegisterHandlersInAssembly(new[] { Assembly.GetAssembly(typeof(APNSSubscriptionsProjection)) }, projectionFactory.Resolve)));

            return cronusSettings;
        }

        private static ICronusSettings UsePorts(this ICronusSettings cronusSettings, Pandora pandora)
        {
            var portFactory = new ServiceLocator(cronusSettings.Container);

            var broker = ConfigurePushBroker(pandora);
            var throttleSettings = new ThrotlleSettings(pandora);
            Func<ISerializer> serializer = () => container.Resolve<ISerializer>();
            var throttler = new ThrottledBrokerAdapter(new ThrottledBroker(cronusSettings.Container, broker, throttleSettings));
            container.RegisterSingleton<IPushBroker>(() => throttler);

            cronusSettings.UsePortConsumer(consumable => consumable
                .WithDefaultPublishers()
                .UseRabbitMqTransport(x =>
                {
                    x.Server = pandora.Get("rabbitmq_server");
                    x.Port = pandora.Get<int>("rabbitmq_port");
                    x.AdminPort = pandora.Get<int>("rabbitmq_admin_port");
                    x.Username = pandora.Get("rabbitmq_username");
                    x.Password = pandora.Get("rabbitmq_password");
                    x.VirtualHost = pandora.Get("rabbitmq_virtualhost");
                })
                .UsePorts(handler => handler.RegisterHandlersInAssembly(new[] { Assembly.GetAssembly(typeof(APNSPort)) }, portFactory.Resolve)));

            return cronusSettings;
        }

        private static PushBroker ConfigurePushBroker(Pandora pandora)
        {
            var broker = new PushBroker();
            broker.OnNotificationSent += PushNotificationLogger.NotificationSent;
            broker.OnChannelException += PushNotificationLogger.ChannelException;
            broker.OnServiceException += PushNotificationLogger.ServiceException;
            broker.OnNotificationFailed += PushNotificationLogger.NotificationFailed;
            broker.OnDeviceSubscriptionExpired += PushNotificationLogger.DeviceSubscriptionExpired;
            broker.OnDeviceSubscriptionChanged += PushNotificationLogger.DeviceSubscriptionChanged;
            broker.OnChannelCreated += PushNotificationLogger.ChannelCreated;
            broker.OnChannelDestroyed += PushNotificationLogger.ChannelDestroyed;

            if (pandora.Get<bool>("pushnot_enable_apns"))
                EnableAPNSNotifications(pandora, broker);

            if (pandora.Get<bool>("pushnot_enable_gcm"))
                EnableGCMNotifications(pandora, broker);

            if (pandora.Get<bool>("pushnot_enable_pushy"))
                EnablePushyNotifications(pandora, broker);

            return broker;
        }

        private static void EnableAPNSNotifications(Pandora pandora, PushBroker broker)
        {
            var iosCert = pandora.Get("pushnot_ios_cert");
            var iosCertPass = pandora.Get("pushnot_ios_cert_pass");
            bool iSprod = Boolean.Parse(pandora.Get("pushnot_ios_production"));

            string iosCertPath = Environment.ExpandEnvironmentVariables(iosCert);
            var appleCert = File.ReadAllBytes(iosCertPath);

            broker.RegisterAppleService(new ApplePushChannelSettings(iSprod, appleCert, iosCertPass, disableCertificateCheck: true));
        }

        private static void EnableGCMNotifications(Pandora pandora, PushBroker broker)
        {
            var androidToken = pandora.Get("pushnot_android_token");

            broker.RegisterGcmService(new GcmPushChannelSettings(androidToken));
        }

        private static void EnablePushyNotifications(Pandora pandora, PushBroker broker)
        {
            var pushyRestApiKey = pandora.Get("pushnot_pushy_rest_api_key");

            broker.RegisterService<PushyNotification>(new PushyNotificationService(pushyRestApiKey));
        }

        private class SessionFactory
        {
            public static Cassandra.ISession Create(string connectionString)
            {
                var cluster = Cassandra.Cluster.Builder()
                    .WithConnectionString(connectionString)
                    .Build();
                var session = cluster.ConnectAndCreateDefaultKeyspaceIfNotExists();
                return session;
            }
        }

        public static void Stop()
        {
            try
            {
                if (host != null)
                {
                    host.Stop();
                    host = null;
                }
                if (container != null)
                {
                    container.Destroy();
                    container = null;
                }
            }
            catch (Exception ex)
            {
                log.FatalException("Unable to stop properly the service.", ex);
                throw;
            }
        }
    }
}
