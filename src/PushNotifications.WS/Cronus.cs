using System;
using System.IO;
using System.Reflection;
using Elders.Cronus;
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
using PushNotifications.Ports;
using PushNotifications.Ports.APNS;
using PushNotifications.Ports.Parse;
using PushNotifications.PushNotifications;
using PushNotifications.WS.Logging;
using PushNotifications.WS.NotificationThrottle;
using PushSharp;
using PushSharp.Android;
using PushSharp.Apple;
using PushSharp.Core;

namespace PushNotifications.WS
{
    public class Cronus
    {
        static ILog log = LogProvider.GetLogger(typeof(Cronus));

        static CronusHost host;
        static Container container;

        public static void Start()
        {
            try
            {
                log.Info("Starting Cronus Push Notifications");

                var appContext = new ApplicationContext("PushNotifications");
                var cfgRepo = new ConsulForPandora(new Uri("http://consul.local.com:8500"));
                var pandora = new Pandora(appContext, cfgRepo);

                string PN = "PushNotifications";

                container = new Container();
                //container.RegisterSingleton<PushBroker>(() => broker);

                container.RegisterSingleton<Cassandra.ISession>(() => SessionCreator.CreateProjectionSession(pandora.Get("pushnot_conn_str_projections")));
                container.RegisterTransient<IRepository>(() => new Repository(
                    new CassandraPersister(container.Resolve<Cassandra.ISession>()),
                    container.Resolve<ISerializer>().SerializeToBytes,
                    container.Resolve<ISerializer>().DeserializeFromBytes));

                var PM_appServiceFactory = new ApplicationServiceFactory(container, PN);
                var cfg = new CronusSettings(container);
                var connstr = pandora.Get("pushnot_conn_str_es");
                log.InfoFormat("ConnectionString => {0}", connstr);
                cfg.UseContractsFromAssemblies(new[] {
                    Assembly.GetAssembly(typeof(PushNotificationWasSent)),
                    Assembly.GetAssembly(typeof(APNSSubscriptionsProjection)),
                    Assembly.GetAssembly(typeof(APNSNotificationMessage))
                });
                cfg.UseCommandConsumer(PN, consumer => consumer
                    .UseRabbitMqTransport(x => (x as IRabbitMqTransportSettings).Server = pandora.Get("pushnot_rabbitmq_server"))
                    .WithDefaultPublishersWithRabbitMq()
                    .UseCassandraEventStore(eventStore => eventStore
                        .SetConnectionString(connstr)
                        .SetAggregateStatesAssembly(typeof(PushNotificationState))
                        .WithNewStorageIfNotExists())
                    .UseApplicationServices(cmdHandler => cmdHandler.RegisterAllHandlersInAssembly(typeof(PushNotificationAppService), PM_appServiceFactory.Create)));

                string PORT = "port";
                var portFactory = new PortHandlerFactory(container);
                cfg.UsePortConsumer(PORT, consumable => consumable
                    .WithDefaultPublishersWithRabbitMq()
                    .UseRabbitMqTransport(x => (x as IRabbitMqTransportSettings).Server = pandora.Get("pushnot_rabbitmq_server"))
                    .UsePorts(handler => handler.RegisterAllHandlersInAssembly(Assembly.GetAssembly(typeof(APNSPort)), portFactory.Create)));

                string PROJ = "proj";
                var projFactory = new PorojectionHandlerFactory(container);
                cfg.UseProjectionConsumer(PROJ, consumable => consumable
                    .WithDefaultPublishersWithRabbitMq()
                    .UseRabbitMqTransport(x => (x as IRabbitMqTransportSettings).Server = pandora.Get("pushnot_rabbitmq_server"))
                    .UseProjections(h => h.RegisterAllHandlersInAssembly(Assembly.GetAssembly(typeof(APNSSubscriptionsProjection)), projFactory.Create)));

                Func<object, byte[]> serializer = container.Resolve<ISerializer>().SerializeToBytes;
                Func<byte[], object> deserializer = container.Resolve<ISerializer>().DeserializeFromBytes;
                var broker = ConfigurePushBroker(pandora);
                var throttleSettings = new ThrotleSettings(pandora);
                var throttler = new ThrottledBrokerAdapter(new ThrottledBroker(serializer, deserializer, broker, throttleSettings));
                container.RegisterSingleton<IPushBroker>(() => throttler);

                (cfg as ISettingsBuilder).Build();
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

            var iosCert = pandora.Get("pushnot_ios_cert");
            var iosCertPass = pandora.Get("pushnot_ios_cert_pass");

            var androidToken = pandora.Get("pushnot_android_token");

            var parseAppId = pandora.Get("pushnot_parse_app_id");
            var parseRestApiKey = pandora.Get("pushnot_parse_rest_api_key");

            string iosCertPath = Environment.ExpandEnvironmentVariables(iosCert);

            var appleCert = File.ReadAllBytes(iosCertPath);

            bool iSprod = Boolean.Parse(pandora.Get("pushnot_ios_production"));
            broker.RegisterAppleService(new ApplePushChannelSettings(iSprod, appleCert, iosCertPass, disableCertificateCheck: true));
            broker.RegisterGcmService(new GcmPushChannelSettings(androidToken));
            broker.RegisterService<ParseAndroidNotifcation>(new ParseNotificationService(parseAppId, parseRestApiKey));

            return broker;
        }

        public class SessionCreator
        {
            public static Cassandra.ISession CreateProjectionSession(string connectionString)
            {
                var cluster = Cassandra.Cluster.Builder()
                    .WithConnectionString(connectionString)
                    .Build();
                var session = cluster.ConnectAndCreateDefaultKeyspaceIfNotExists();
                session.InitializeProjectionDatabase(Assembly.GetAssembly(typeof(APNSSubscriptionsProjection)));
                return session;
            }
        }

        public class ApplicationServiceFactory
        {
            private readonly IContainer container;
            private readonly string namedInstance;

            public ApplicationServiceFactory(IContainer container, string namedInstance)
            {
                this.container = container;
                this.namedInstance = namedInstance;
            }

            public object Create(Type appServiceType)
            {
                var appService = FastActivator
                    .CreateInstance(appServiceType);
                return appService;
            }
        }

        public class PortHandlerFactory
        {
            private readonly IContainer container;

            public PortHandlerFactory(IContainer container)
            {
                this.container = container;
            }

            public object Create(Type handlerType)
            {
                var handler = FastActivator
                    .CreateInstance(handlerType)
                    .AssignPropertySafely<IPushNotificationPort>(x => x.PushBroker = container.Resolve<IPushBroker>())
                    .AssignPropertySafely<IHaveProjectionsRepository>(x => x.Repository = container.Resolve<IRepository>());
                return handler;
            }
        }

        public class PorojectionHandlerFactory
        {
            private readonly IContainer container;

            public PorojectionHandlerFactory(IContainer container)
            {
                this.container = container;
            }

            public object Create(Type handlerType)
            {
                var handler = FastActivator
                    .CreateInstance(handlerType)
                    .AssignPropertySafely<IHaveProjectionsRepository>(x => x.Repository = container.Resolve<IRepository>());
                return handler;
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
