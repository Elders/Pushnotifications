using Elders.Cronus;
using Elders.Cronus.IocContainer;
using System;
using System.Linq;
using System.Reflection;

namespace Facilities.Factory
{
    public class ServiceLocator
    {
        private IContainer container;

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
}
