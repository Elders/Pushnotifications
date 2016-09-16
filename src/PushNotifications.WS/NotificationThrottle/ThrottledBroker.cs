using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Elders.Cronus.Pipeline;
using PushSharp.Core;
using Elders.Cronus.DomainModeling;
using Elders.Cronus.Pipeline.Transport.RabbitMQ;
using Elders.Multithreading.Scheduler;
using PushNotifications.Ports.Parse;
using PushSharp.Android;
using PushSharp.Apple;

namespace PushNotifications.WS.NotificationThrottle
{
    public class ThrottledBroker : IDisposable
    {
        private readonly Func<byte[], object> deserialize;
        private WorkPool pool;
        private RabbitMqTransport rabbitMqTransport;
        private readonly Func<object, byte[]> serialize;

        public ThrottledBroker(Func<object, byte[]> serialize, Func<byte[], object> deserialize, IPushBroker broker)
        {
            this.deserialize = deserialize;
            this.serialize = serialize;
            rabbitMqTransport = new RabbitMqTransport(new ThrotleSettings());
            var endpointDefinition = rabbitMqTransport.EndpointFactory.GetEndpointDefinition(null).Single();
            var endpoint = rabbitMqTransport.EndpointFactory.CreateEndpoint(endpointDefinition);
            pool = new WorkPool("Throtler", 1);
            pool.AddWork(new MessagePublishingWork(endpoint, deserialize, broker));
            pool.StartCrawlers();
        }

        public void QueueNotification<TPushNotification>(TPushNotification notification) where TPushNotification : Notification
        {
            var throttleMessage = ToThrottle(notification);
            var messageType = throttleMessage.GetType();
            var pipeline = rabbitMqTransport.PipelineFactory.GetPipeline(messageType);
            var endpointMessage = new EndpointMessage(serialize(throttleMessage), "", new Dictionary<string, object>() { { messageType.GetAttrubuteValue<DataContractAttribute, string>(x => x.Name), string.Empty } });
            System.Diagnostics.Trace.WriteLine("Sending notification with lenght:" + endpointMessage.Body.Length);
            pipeline.Push(endpointMessage);
        }

        private IThrottleNotification ToThrottle(Notification notification)
        {
            if (notification is AppleNotification)
            {
                return new APNSNotificationMessage(notification as AppleNotification);
            }
            if (notification is GcmNotification)
            {
                return new GCMNotificationMessage(notification as GcmNotification);
            }
            if (notification is ParseAndroidNotifcation)
            {
                return new ParseNotificationMessage(notification as ParseAndroidNotifcation);
            }
            throw new NotSupportedException(notification.GetType().ToString());
        }

        public DateTime ScheduledStart { get; private set; }

        public class MessagePublishingWork : IWork
        {
            private readonly IPushBroker broker;
            private readonly Func<byte[], object> deserizalize;
            private readonly IEndpoint endpoint;
            static log4net.ILog log = log4net.LogManager.GetLogger(typeof(MessagePublishingWork));
            ProcessorMonitor monitor;

            public MessagePublishingWork(IEndpoint endpoint, Func<byte[], object> deserizalize, IPushBroker broker)
            {
                this.broker = broker;
                this.deserizalize = deserizalize;
                this.endpoint = endpoint;
                monitor = new ProcessorMonitor(30, new TimeSpan(0, 0, 5));
            }

            public void Start()
            {
                var scheduleSeconds = 5;
                try
                {
                    var usage = monitor.Usage();

                    if (usage < 50)
                    {
                        endpoint.Open();
                        using (var sender = new NotificationSender(broker))
                        {
                            for (int i = 0; i < 20; i++)
                            {
                                EndpointMessage message;
                                endpoint.BlockDequeue(20, out message);
                                if (message != null)
                                {
                                    var notification = deserizalize(message.Body) as IThrottleNotification;
                                    if (notification == null)
                                    {
                                        log.Error("Can not deserialize notification" + notification);
                                        continue;
                                    }
                                    sender.SendNotification((notification as IThrottleNotification).ToNotification());
                                }
                                else
                                {
                                    scheduleSeconds = 5;
                                    break;
                                }
                            }
                        }
                        ScheduledStart = DateTime.UtcNow.AddSeconds(scheduleSeconds);
                    }
                    else
                    {
                        log.Warn("Usage above 50%, not sending notifications. Usage: " + usage + "%");
                    }
                }
                catch (EndpointClosedException ex)
                {
                    log.Warn("Endpoint Closed", ex);
                }
                catch (Exception ex)
                {
                    log.Error("Unexpected Exception.", ex);
                }
                finally
                {
                    try
                    {
                        endpoint.AcknowledgeAll();
                        endpoint.Close();
                    }
                    catch (Exception ex)
                    {
                        log.Error("Could not close the endpoint.", ex);
                    }
                    ScheduledStart = DateTime.UtcNow.AddSeconds(scheduleSeconds);
                }
            }

            public void Stop() { }

            public DateTime ScheduledStart { get; private set; }
        }

        public void Dispose()
        {
            if (ReferenceEquals(null, pool) == false)
            {
                pool.Stop();
                pool = null;
            }
            if (ReferenceEquals(null, rabbitMqTransport) == false)
            {
                rabbitMqTransport.Dispose();
                rabbitMqTransport = null;
            }
        }
    }
}
