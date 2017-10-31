using System;
using Elders.Cronus;
using Elders.Cronus.DomainModeling;
using Elders.Cronus.IocContainer;

namespace PushNotifications.WS
{
    public class ApplicationServiceFactory
    {
        readonly IContainer container;
        readonly string namedInstance;

        public ApplicationServiceFactory(IContainer container, string namedInstance)
        {
            this.container = container;
            this.namedInstance = namedInstance;
        }

        public object Create(Type appServiceType)
        {
            var appService = FastActivator
                .CreateInstance(appServiceType)
                .AssignPropertySafely<IAggregateRootApplicationService>(x => x.Repository = container.Resolve<IAggregateRepository>(namedInstance));
            return appService;
        }
    }
}
