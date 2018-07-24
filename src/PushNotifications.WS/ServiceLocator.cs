using System;
using System.Linq;
using System.Reflection;
using Elders.Cronus;
using Elders.Cronus.IocContainer;

namespace PushNotifications.WS
{
    public class ServiceLocator
    {
        IContainer container;
        private readonly string namedInstance;

        public ServiceLocator(IContainer container, string namedInstance = null)
        {
            this.container = container;
            this.namedInstance = namedInstance;
        }

        public object Resolve(Type objectType)
        {
            var instance = FastActivator.CreateInstance(objectType);
            var props = objectType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).ToList();

            var globalDependencies = props.Where(x => container.IsRegistered(x.PropertyType));
            foreach (var item in globalDependencies)
            {
                item.SetValue(instance, container.Resolve(item.PropertyType));
            }

            var dependencies = props.Where(x => container.IsRegistered(x.PropertyType, namedInstance));
            foreach (var item in dependencies)
            {
                item.SetValue(instance, container.Resolve(item.PropertyType, namedInstance));
            }
            return instance;
        }

        public T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }
    }
}
