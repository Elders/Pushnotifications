using System;
using Elders.Cronus.Pipeline.Config;
using Elders.Cronus.Pipeline.Hosts;
using Elders.Cronus.Pipeline.Transport.RabbitMQ.Config;
using Elders.Cronus.IocContainer;
using Elders.Pandora;
using Elders.Cronus.DomainModeling;
using System.Web.Http;
using Elders.Cronus.Pipeline;
using Elders.Cronus.Pipeline.Transport;
using Elders.Cronus.Serializer;
using System.Reflection;
using PushNotifications.Contracts.PushNotifications.Events;

namespace PushNotifications.Api
{
    public static class Cronus
    {
        private static CronusHost host;
        static log4net.ILog log;
        static Container container;

        public static void UseCronusCommandPublisher(this HttpConfiguration apiConfig)
        {
            try
            {
                log4net.Config.XmlConfigurator.Configure();
                log = log4net.LogManager.GetLogger(typeof(Cronus));
                log.Info("Starting Cronus Push Notifications Api");

                ApplicationConfiguration.SetContext("PushNotifications");

                container = new Container();
                var cfg = new CronusSettings(container);

                cfg.UseContractsFromAssemblies(new[] { Assembly.GetAssembly(typeof(PushNotificationWasSent)) });
                cfg.UseRabbitMqTransport(x => (x as IRabbitMqTransportSettings).Server = ApplicationConfiguration.Get("pushnot_rabbitmq_server"));

                Func<IPipelineTransport> transport = () => container.Resolve<IPipelineTransport>();
                Func<ISerializer> serializer = () => container.Resolve<ISerializer>();
                container.RegisterSingleton<IPublisher<ICommand>>(() => new PipelinePublisher<ICommand>(transport(), serializer()));

                (cfg as ISettingsBuilder).Build();
                host = container.Resolve<CronusHost>();
                host.Start();
                log.Info("STARTED Cronus Push Notifications Api");

                //  Not related with cronus...
                apiConfig.Services.Replace(typeof(System.Web.Http.Dispatcher.IHttpControllerActivator), new CustomHttpControllerActivator(container));
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw;
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
