using Elders.Cronus.IocContainer;
using System;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Controllers;
using Elders.Cronus;
using System.Reflection;

namespace PushNotifications.Api.Host
{
    public class ServiceLocator
    {
        IContainer container;

        public ServiceLocator(IContainer container)
        {
            this.container = container;
        }

        public object Resolve(Type objectType)
        {
            var instance = FastActivator.CreateInstance(objectType);
            var props = objectType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).ToList();
            var dependencies = props.Where(x => container.IsRegistered(x.PropertyType));
            foreach (var item in dependencies)
            {
                item.SetValue(instance, container.Resolve(item.PropertyType));
            }
            return instance;
        }

        public T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }
    }

    public class ServiceLocatorFactory : System.Web.Http.Dispatcher.IHttpControllerActivator
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
