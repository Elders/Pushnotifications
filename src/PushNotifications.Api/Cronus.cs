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
using PushNotifications.Api.Logging;

namespace PushNotifications.Api
{
    public static class Cronus
    {
        static ILog log = LogProvider.GetLogger(typeof(Cronus));

        static CronusHost host;
        static Container container;

        public static void UseCronusCommandPublisher(this HttpConfiguration apiConfig, Pandora pandora)
        {
            try
            {
                log.Info("Starting Cronus Push Notifications Api");

                container = new Container();
                var cfg = new CronusSettings(container);
                container.RegisterSingleton<Pandora>(() => pandora);
                cfg.UseContractsFromAssemblies(new[] { Assembly.GetAssembly(typeof(PushNotificationWasSent)) });
                cfg.UseRabbitMqTransport(x => (x as IRabbitMqTransportSettings).Server = pandora.Get("pushnot_rabbitmq_server"));

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
                log.ErrorException("Unable to boot PushNotifications.Api", ex);
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
