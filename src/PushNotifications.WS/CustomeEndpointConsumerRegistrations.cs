using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cassandra;
using Elders.Cronus;
using Elders.Cronus.AtomicAction.Config;
using Elders.Cronus.AtomicAction.Redis.Config;
using Elders.Cronus.Cluster.Config;
using Elders.Cronus.DomainModeling;
using Elders.Cronus.IocContainer;
using Elders.Cronus.Persistence.Cassandra.Config;
using Elders.Cronus.Pipeline.Config;
using Elders.Cronus.Pipeline.Hosts;
using Elders.Cronus.Pipeline.Transport.RabbitMQ.Config;
using Elders.Pandora;
using PushNotifications;
using PushNotifications.Contracts;
using PushNotifications.Projections;
using PushNotifications.WS.Logging;

namespace PushNotifications.WS
{
    public static class CustomeEndpointConsumerRegistrations
    {
        public static T RegisterHandlerTypes<T>(this T self, IEnumerable<Type> messageHandlers, Func<Type, object> messageHandlerFactory) where T : ISubscrptionMiddlewareSettings
        {
            self.HandlerRegistrations = messageHandlers.ToList();
            self.HandlerFactory = messageHandlerFactory;
            return self;
        }
    }
}