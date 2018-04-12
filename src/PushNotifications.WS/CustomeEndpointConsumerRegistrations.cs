using System;
using System.Collections.Generic;
using System.Linq;
using Elders.Cronus.Pipeline.Config;

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
