using Elders.Cronus.AtomicAction.Config;
using Elders.Cronus.AtomicAction.Redis.Config;
using Elders.Cronus.Cluster.Config;
using Elders.Cronus.DomainModeling;
using Elders.Cronus.IocContainer;
using Elders.Cronus.Pipeline;
using Elders.Cronus.Pipeline.Config;
using Elders.Cronus.Pipeline.Hosts;
using Elders.Cronus.Pipeline.Transport;
using Elders.Cronus.Pipeline.Transport.RabbitMQ.Config;
using Elders.Cronus.Serializer;
using Elders.Pandora;
using Facilities.Factory;
using PushNotifications.Contracts.PushNotifications.Events;
using System;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace PushNotifications.Api
{
    public class PushNotificationsApiConfiguration : IApiConfiguration
    {
        public Assembly Assembly { get { return typeof(PushNotificationsApiConfiguration).Assembly; } }

        public IHttpControllerActivator ControllerFactory { get; private set; }

        public void Initialize(HttpConfiguration configuration, Pandora pandora)
        {
            var container = new Container();
            container.RegisterSingleton<Pandora>(() => pandora);
            Func<IPipelineTransport> transport = () => container.Resolve<IPipelineTransport>();
            Func<ISerializer> serializer = () => container.Resolve<ISerializer>();
            container.RegisterSingleton<IPublisher<ICommand>>(() => new PipelinePublisher<ICommand>(transport(), serializer()));

            var serviceLocator = new ServiceLocator(container);
            ControllerFactory = new ServiceLocatorFactory(serviceLocator);

            var cfg = new CronusSettings(container)
                  .UseCluster(cluster =>
                       cluster.UseAggregateRootAtomicAction(atomic =>
                       {
                           if (pandora.Get<bool>("enable_redis_atomic_action"))
                               atomic.UseRedis(redis => redis.SetLockEndPoints(pandora.Get("redis_endpoints").ParseIPEndPoints(";")));
                           else
                               atomic.WithInMemory();
                       }))
                  .UseContractsFromAssemblies(new[] { Assembly.GetAssembly(typeof(PushNotificationWasSent)) })
                  .UseRabbitMqTransport(x => (x as IRabbitMqTransportSettings).Server = pandora.Get("pushnot_rabbitmq_server"));

            (cfg as ICronusSettings).Build();
        }
    }

    public class ServiceLocatorFactory : IHttpControllerActivator
    {
        ServiceLocator locator;

        public ServiceLocatorFactory(ServiceLocator locator)
        {
            this.locator = locator;
        }

        public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType)
        {
            return locator.Resolve(controllerType) as IHttpController;
        }
    }
}
