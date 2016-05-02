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
using PushSharp;
using PushSharp.Android;
using PushSharp.Apple;

namespace PushNotifications.WS
{
    public class Cronus
    {
        private static CronusHost host;
        static log4net.ILog log;
        static Container container;

        public static void Start()
        {
            try
            {
                log4net.Config.XmlConfigurator.Configure();
                log = log4net.LogManager.GetLogger(typeof(Cronus));
                log.Info("Starting Cronus Push Notifications");

                ApplicationConfiguration.SetContext("PushNotifications");

                string PN = "PushNotifications";

                container = new Container();
                container.RegisterSingleton<PushBroker>(() => ConfigurePushBroker());
                container.RegisterSingleton<Cassandra.ISession>(() => SessionCreator.CreateProjectionSession());
                container.RegisterTransient<IRepository>(() => new Repository(
                    new CassandraPersister(container.Resolve<Cassandra.ISession>()),
                    container.Resolve<ISerializer>().SerializeToBytes,
                    container.Resolve<ISerializer>().DeserializeFromBytes));

                var PM_appServiceFactory = new ApplicationServiceFactory(container, PN);
                var cfg = new CronusSettings(container);
                var connstr = ApplicationConfiguration.Get("pushnot_conn_str_es");
                log.InfoFormat("ConnectionString => {0}", connstr);
                cfg.UseContractsFromAssemblies(new[] { Assembly.GetAssembly(typeof(PushNotificationWasSent)), Assembly.GetAssembly(typeof(APNSSubscriptionsProjection)) });
                cfg.UseCommandConsumer(PN, consumer => consumer
                    .UseRabbitMqTransport(x => (x as IRabbitMqTransportSettings).Server = ApplicationConfiguration.Get("pushnot_rabbitmq_server"))
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
                    .UseRabbitMqTransport(x => (x as IRabbitMqTransportSettings).Server = ApplicationConfiguration.Get("pushnot_rabbitmq_server"))
                    .UsePorts(handler => handler.RegisterAllHandlersInAssembly(Assembly.GetAssembly(typeof(APNSPort)), portFactory.Create)));

                string PROJ = "proj";
                var projFactory = new PorojectionHandlerFactory(container);
                cfg.UseProjectionConsumer(PROJ, consumable => consumable
                    .WithDefaultPublishersWithRabbitMq()
                    .UseRabbitMqTransport(x => (x as IRabbitMqTransportSettings).Server = ApplicationConfiguration.Get("pushnot_rabbitmq_server"))
                    .UseProjections(h => h.RegisterAllHandlersInAssembly(Assembly.GetAssembly(typeof(APNSSubscriptionsProjection)), projFactory.Create)));

                (cfg as ISettingsBuilder).Build();
                host = container.Resolve<CronusHost>();
                host.Start();
                log.Info("STARTED Cronus Push Notifications");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw;
            }
        }

        private static PushBroker ConfigurePushBroker()
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

            var iosCert = ApplicationConfiguration.Get("pushnot_ios_cert");
            var iosCertPass = ApplicationConfiguration.Get("pushnot_ios_cert_pass");

            var androidToken = ApplicationConfiguration.Get("pushnot_android_token");

            var parseAppId = ApplicationConfiguration.Get("pushnot_parse_app_id");
            var parseRestApiKey = ApplicationConfiguration.Get("pushnot_parse_rest_api_key");

            string iosCertPath = Environment.ExpandEnvironmentVariables(iosCert);

            var appleCert = File.ReadAllBytes(iosCertPath);

            bool iSprod = Boolean.Parse(ApplicationConfiguration.Get("pushnot_ios_production"));
            broker.RegisterAppleService(new ApplePushChannelSettings(iSprod, appleCert, iosCertPass));
            broker.RegisterGcmService(new GcmPushChannelSettings(androidToken));
            broker.RegisterService<ParseAndroidNotifcation>(new ParseNotificationService(parseAppId, parseRestApiKey));

            return broker;
        }

        public class SessionCreator
        {
            public static Cassandra.ISession CreateProjectionSession()
            {
                var cluster = Cassandra.Cluster.Builder()
                    .WithConnectionString(ApplicationConfiguration.Get("pushnot_conn_str_projections"))
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
                    .AssignPropertySafely<IPushNotificationPort>(x => x.PushBroker = container.Resolve<PushBroker>())
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
                log.Fatal("Unable to stop properly the service.", ex);
                throw;
            }
        }
    }
}
