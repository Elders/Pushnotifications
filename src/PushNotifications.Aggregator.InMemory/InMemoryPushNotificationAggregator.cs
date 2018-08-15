using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using PushNotifications.Aggregator.InMemory.Logging;
using PushNotifications.Contracts;
using PushNotifications.Contracts.PushNotifications.Delivery;

namespace PushNotifications.Aggregator.InMemory
{
    public class InMemoryPushNotificationAggregator : IDisposable, IPushNotificationAggregator
    {
        static ILog log = LogProvider.GetLogger(typeof(InMemoryPushNotificationAggregator));

        private readonly object _lockBuffer = new object();

        Func<List<SubscriptionToken>, NotificationForDelivery, SendTokensResult> send;

        readonly TimeSpan timeSpanBeforeFlush;

        readonly int recipientsCountBeforeFlush;

        Timer timer;

        bool canSend;

        Dictionary<NotificationForDelivery, List<SubscriptionToken>> buffer;

        /// <summary>
        /// Aggregates push notifications and flushes them as single batch
        /// Aggregation is based on <see cref="NotificationForDelivery"/>
        /// Flushing is based on time span and number of recipients
        /// </summary>
        /// <param name="timeSpanBeforeFlush">Time span on which flushing is happening</param>
        /// <param name="recipientsCountBeforeFlush">Number of recipients before flushing</param>
        public InMemoryPushNotificationAggregator(Func<List<SubscriptionToken>, NotificationForDelivery, SendTokensResult> send, TimeSpan timeSpanBeforeFlush, int recipientsCountBeforeFlush)
        {
            if (ReferenceEquals(null, send) == true) throw new ArgumentNullException(nameof(send));
            if (ReferenceEquals(null, timeSpanBeforeFlush) == true) throw new ArgumentNullException(nameof(timeSpanBeforeFlush));

            this.send = send;
            this.timeSpanBeforeFlush = timeSpanBeforeFlush;
            this.recipientsCountBeforeFlush = recipientsCountBeforeFlush;
            canSend = true;
            buffer = new Dictionary<NotificationForDelivery, List<SubscriptionToken>>(new NotificationForDeliveryEqualityComparer());
            timer = new Timer(x => FlushAll(), 1, TimeSpan.FromSeconds(0), timeSpanBeforeFlush);
        }

        public SendTokensResult Queue(SubscriptionToken token, NotificationForDelivery notification)
        {
            if (ReferenceEquals(null, token) == true) throw new ArgumentNullException(nameof(token));
            if (ReferenceEquals(null, notification) == true) throw new ArgumentNullException(nameof(notification));

            if (timeSpanBeforeFlush == TimeSpan.Zero || recipientsCountBeforeFlush == 1)
                return send(new List<SubscriptionToken> { token }, notification);

            if (canSend == false)
            {
                log.Error($"The InMemoryPushNotificationAggregator cannot send. Token {token} and notification {notification.NotificationPayload}");
                return SendTokensResult.Failed;
            }

            lock (_lockBuffer)
            {
                List<SubscriptionToken> tokens;
                if (buffer.TryGetValue(notification, out tokens))
                {
                    tokens.Add(token);
                    if (tokens.Count >= recipientsCountBeforeFlush)
                    {
                        if (buffer.Remove(notification))
                            return send(tokens, notification);
                    }
                }
                else
                {
                    buffer.Add(notification, new List<SubscriptionToken>() { token });
                }
            }

            return SendTokensResult.Success;
        }

        SendTokensResult Flush(NotificationForDelivery notification)
        {
            lock (_lockBuffer)
            {
                List<SubscriptionToken> tokens;
                if (buffer.TryGetValue(notification, out tokens))
                {
                    if (buffer.Remove(notification))
                        return send(tokens, notification);
                    else
                        log.Error($"Failed to remove notification from the {nameof(InMemoryPushNotificationAggregator)} buffer. Most likely the access to the {nameof(buffer)} is not properly synchronized. I suggest you to delete this class and rewrite it.");
                }
            }

            return SendTokensResult.Success;
        }

        void FlushAll()
        {
            log.Debug($"Flushing the aggregator, currently has {buffer.Keys.Count} pending notifications");

            foreach (var notification in buffer.Keys.ToList())
            {
                Flush(notification);
            }
        }

        public void Dispose()
        {
            log.Debug("Disposing the aggregator");

            canSend = false;
            FlushAll();

            timer?.Dispose();
            timer = null;
        }

        struct NotificationForDeliveryEqualityComparer : IEqualityComparer<NotificationForDelivery>
        {
            public bool Equals(NotificationForDelivery x, NotificationForDelivery y)
            {
                return x.Id.Equals(y.Id);
            }

            public int GetHashCode(NotificationForDelivery obj)
            {
                return obj.Id.GetHashCode();
            }
        }
    }
}
