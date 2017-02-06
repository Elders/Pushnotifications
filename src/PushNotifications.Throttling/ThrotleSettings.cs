using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Elders.Cronus.Pipeline;
using Elders.Cronus.Pipeline.Transport.RabbitMQ.Config;
using Elders.Cronus.DomainModeling;
using Elders.Pandora;
using Elders.Cronus.MessageProcessing;
using PushNotifications.APNS;
using PushNotifications.GCM;
using PushNotifications.Pushy;

namespace PushNotifications.Throttling
{
    public class ThrotlleSettings : IRabbitMqTransportSettings
    {
        public ThrotlleSettings(Pandora pandora)
        {
            Server = pandora.Get("rabbitmq_server");
            Port = pandora.Get<int>("rabbitmq_port");
            AdminPort = pandora.Get<int>("rabbitmq_admin_port");
            Username = pandora.Get("rabbitmq_username");
            Password = pandora.Get("rabbitmq_password");
            VirtualHost = pandora.Get("rabbitmq_virtualhost");
            PushNotificationsBatchSize = pandora.Get<int>("pushnot_batch_size");
            PushNotificationsSendoutDelay = pandora.Get<int>("pushnot_sendout_delay");
            PushNotificationsMaxCPUUtilization = pandora.Get<int>("pushnot_max_cpu_utilization");
            EndpointNameConvention = new ThrottledBrokerEndpointNameConvention(typeof(APNSNotificationMessage), typeof(GCMNotificationMessage), typeof(PushyNotificationMessage));
            PipelineNameConvention = new ThrottledBrokerPipelineNameConvention();
        }

        public string Name { get; set; }

        public string Password { get; set; }

        public int Port { get; set; }

        public int AdminPort { get; set; }

        public string Server { get; set; }

        public string Username { get; set; }

        public string VirtualHost { get; set; }

        public int PushNotificationsBatchSize { get; set; }

        public int PushNotificationsSendoutDelay { get; set; }

        public int PushNotificationsMaxCPUUtilization { get; set; }

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

            public IEnumerable<EndpointDefinition> GetEndpointDefinition(IEndpointConsumer consumer, SubscriptionMiddleware subscriptionMiddleware)
            {
                yield return endpoint;
            }
        }
    }
}
