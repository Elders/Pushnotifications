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
using Consul;
using PushNotifications.Api.Discovery;

namespace PushNotifications.Api
{
    public class PushNotificationsApiConfiguration : IApiConfiguration, IAmDiscoverable
    {
        static Uri consulUri = new Uri("http://consul.local.com:8500");

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
                  .UseRabbitMqTransport(x =>
                  {
                      x.Server = pandora.Get("rabbitmq_server");
                      x.Username = pandora.Get("rabbitmq_username");
                      x.Password = pandora.Get("rabbitmq_password");
                      x.VirtualHost = pandora.Get("rabbitmq_virtualhost");
                  });

            (cfg as ICronusSettings).Build();
        }

        public void RegisterServices(HttpConfiguration configuration, Pandora pandora)
        {
            var baseUri = new Uri(pandora.Get("pn_base_url"));
            var consulClient = new ConsulClient(x => x.Address = consulUri);
            var consulEndpointRegistrationService = new ConsulEndpointRegistrationService(consulClient);
            consulEndpointRegistrationService.RegisterServices(configuration, Assembly, baseUri);
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
