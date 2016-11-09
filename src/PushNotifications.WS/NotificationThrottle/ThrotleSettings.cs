using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Elders.Cronus.Pipeline;
using Elders.Cronus.Pipeline.Transport.RabbitMQ.Config;
using Elders.Cronus.DomainModeling;
using Elders.Pandora;

namespace PushNotifications.WS.NotificationThrottle
{
    public class ThrotleSettings : IRabbitMqTransportSettings
    {
        public ThrotleSettings(Pandora pandora)
        {
            Server = pandora.Get("pushnot_rabbitmq_server");
            Port = pandora.Get<int>("pushnot_rabbitmq_port");
            Username = pandora.Get("pushnot_rabbitmq_username");
            Password = pandora.Get("pushnot_rabbitmq_password");
            VirtualHost = pandora.Get("pushnot_rabbitmq_virtual_host");
            EndpointNameConvention = new ThrottledBrokerEndpointNameConvention(typeof(APNSNotificationMessage), typeof(GCMNotificationMessage), typeof(ParseNotificationMessage));
            PipelineNameConvention = new ThrottledBrokerPipelineNameConvention();
        }

        public string Name { get; set; }

        public string Password { get; set; }

        public int Port { get; set; }

        public string Server { get; set; }

        public string Username { get; set; }

        public string VirtualHost { get; set; }

        public IEndpointNameConvention EndpointNameConvention { get; set; }

        public IPipelineNameConvention PipelineNameConvention { get; set; }

        public Elders.Cronus.IocContainer.IContainer Container { get; set; }

        public void Build()
        {

        }

        public class ThrottledBrokerPipelineNameConvention : IPipelineNameConvention
        {
            public string GetPipelineName(Type messageType)
            {
                return "PushNotification.Throttle";
            }
        }

        public class ThrottledBrokerEndpointNameConvention : IEndpointNameConvention
        {
            EndpointDefinition endpoint;
            public ThrottledBrokerEndpointNameConvention(params Type[] messageTypes)
            {
                var headers = messageTypes.Select(x => x.GetAttrubuteValue<DataContractAttribute, string>(y => y.Name)).ToDictionary<string, string, object>(x => x, x => string.Empty);
                endpoint = new EndpointDefinition(new ThrottledBrokerPipelineNameConvention().GetPipelineName(null),
                   "PushNotification.Throttle.Queue",
                   headers);
            }

            public IEnumerable<EndpointDefinition> GetEndpointDefinition(Elders.Cronus.IMessageProcessor messageProcessor)
            {
                yield return endpoint;
            }
        }
    }
}
