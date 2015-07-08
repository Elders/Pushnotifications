using System;
using System.Reflection;
using Elders.Cronus.Pipeline.Config;
using Elders.Cronus.Pipeline.Hosts;
using Elders.Cronus.Pipeline.Transport.RabbitMQ.Config;
using Elders.Cronus.IocContainer;
using Elders.Cronus.Persistence.Cassandra.Config;
using Elders.Cronus;
using Elders.Pandora;
using System.Linq;
using PushNotifications.Contracts.PushNotifications.Events;
using PushNotifications.PushNotifications;
using PushNotifications.Ports;
using PushSharp;
using System.IO;
using PushSharp.Apple;
using PushSharp.Core;
using Elders.Cronus.Serializer;
using Elders.Cronus.DomainModeling.Projections;
using Elders.Cronus.Projections.Cassandra;

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
        static PushBroker ConfigurePushBroker()
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
            //var androidToken = ApplicationConfiguration.Get("pushnot_android_token");

            //var parseAppId = ApplicationConfiguration.Get("pushnot_parse_app_id");
            //var parseRestApiKey = ApplicationConfiguration.Get("pushnot_parse_rest_api_key");

            var appleCert = File.ReadAllBytes(iosCert);

            bool iSprod = Boolean.Parse(ApplicationConfiguration.Get("pushnot_ios_production"));
            broker.RegisterAppleService(new ApplePushChannelSettings(iSprod, appleCert, iosCertPass));
            //broker.RegisterService<ParseAndroidNotifcation>(new ParseNotificationService(parseAppId, parseRestApiKey));
            //broker.RegisterGcmService(new GcmPushChannelSettings(androidToken));

            //  Test android
            //var notification = new GcmNotification()
            //             .ForDeviceRegistrationId("APA91bESWcOJaSlTJhrJ0ll6OrL0uIh0dQV_PvOfSCtoCeQiNwhmBk4C5BMgsGROX_X4nq6nqHQFnpfWHbyk-fkTj-DpTcLQkrmrO_hpLGTZYNhUNfQo8WuCTk80mQ1m1uq4LWL5Js3odOGewYzuxM9tvRm9aqD1wQ")
            //             .WithJson("{\"isMessage\": true ,\"message\":" + "\"test\"" + "}");
            //broker.QueueNotification(notification);
            //Console.ReadLine();
            return broker;
        }
        public static class PushNotificationLogger
        {
            static log4net.ILog log = log4net.LogManager.GetLogger(typeof(PushNotificationLogger));
            public static string TOKEN;
            public static void DeviceSubscriptionChanged(object sender, string oldSubscriptionId, string newSubscriptionId, INotification notification)
            {
                //Currently this event will only ever happen for Android GCM
                log.Info("Device Registration Changed:  Old-> " + oldSubscriptionId + "  New-> " + newSubscriptionId + " -> " + notification);
            }

            public static void NotificationSent(object sender, INotification notification)
            {
                log.Info("Sent: " + sender + " -> " + notification);
            }

            public static void NotificationFailed(object sender, INotification notification, Exception notificationFailureException)
            {
                log.Error("Failure: " + sender + " -> " + notification + "TOKEN:" + TOKEN, notificationFailureException);
            }

            public static void ChannelException(object sender, IPushChannel channel, Exception exception)
            {
                log.Error("Channel Exception: " + sender, exception);
            }

            public static void ServiceException(object sender, Exception exception)
            {
                log.Error("Channel Exception: " + sender, exception);
            }

            public static void DeviceSubscriptionExpired(object sender, string expiredDeviceSubscriptionId, DateTime timestamp, INotification notification)
            {
                log.Warn("Device Subscription Expired: " + sender + " -> " + expiredDeviceSubscriptionId);
            }

            public static void ChannelDestroyed(object sender)
            {
                log.Warn("Channel Destroyed for: " + sender);
            }

            public static void ChannelCreated(object sender, IPushChannel pushChannel)
            {
                log.Info("Channel Created for: " + sender);
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
                    .AssignPropertySafely<IAPNSPort>(x => x.PushBroker = container.Resolve<PushBroker>())
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
            host.Stop();
            host = null;
            container.Destroy();
        }
    }
}
