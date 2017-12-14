using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using PushNotifications.Aggregator.InMemory.Logging;
using PushNotifications.Contracts;
using PushNotifications.Contracts.PushNotifications.Delivery;

namespace PushNotifications.Aggregator.InMemory
{
    public class InMemoryPushNotificationAggregator : IDisposable, IPushNotificationAggregator
    {
        Func<List<SubscriptionToken>, NotificationForDelivery, bool> send;

        readonly TimeSpan timeSpanBeforeFlush;

        readonly int recipientsCountBeforeFlush;

        Timer timer;

        bool canSend;

        ConcurrentDictionary<NotificationForDelivery, List<SubscriptionToken>> buffer;

        static ILog log = LogProvider.GetLogger(typeof(InMemoryPushNotificationAggregator));

        /// <summary>
        /// Aggregates push notifications and flushes them as single batch
        /// Aggregation is based on <see cref="NotificationForDelivery"/>
        /// Flushing is based on time span and number of recipients
        /// </summary>
        /// <param name="timeSpanBeforeFlush">Time span on which flushing is happening</param>
        /// <param name="recipientsCountBeforeFlush">Number of recipients before flushing</param>
        public InMemoryPushNotificationAggregator(Func<List<SubscriptionToken>, NotificationForDelivery, bool> send, TimeSpan timeSpanBeforeFlush, int recipientsCountBeforeFlush)
        {
            if (ReferenceEquals(null, send) == true) throw new ArgumentNullException(nameof(send));
            if (ReferenceEquals(null, timeSpanBeforeFlush) == true) throw new ArgumentNullException(nameof(timeSpanBeforeFlush));

            this.send = send;
            this.timeSpanBeforeFlush = timeSpanBeforeFlush;
            this.recipientsCountBeforeFlush = recipientsCountBeforeFlush;
            canSend = true;
            buffer = new ConcurrentDictionary<NotificationForDelivery, List<SubscriptionToken>>();
            timer = new Timer(x => Flush(), this, TimeSpan.FromSeconds(0), timeSpanBeforeFlush);
        }

        public bool Queue(SubscriptionToken token, NotificationForDelivery notification)
        {
            if (ReferenceEquals(null, token) == true) throw new ArgumentNullException(nameof(token));
            if (ReferenceEquals(null, notification) == true) throw new ArgumentNullException(nameof(notification));
            if (canSend == false)
                return false;

            if (timeSpanBeforeFlush == TimeSpan.Zero || recipientsCountBeforeFlush == 1)
                return send(new List<SubscriptionToken> { token }, notification);

            buffer.AddOrUpdate(notification, new List<SubscriptionToken> { token }, (k, v) => { v.Add(token); return v; });

            List<SubscriptionToken> tokens;
            if (buffer.TryGetValue(notification, out tokens) && tokens.Count >= recipientsCountBeforeFlush)
                return Flush();

            return true;
        }

        bool Flush()
        {
            foreach (var key in buffer.Keys)
            {
                List<SubscriptionToken> tokens;
                if (buffer.TryRemove(key, out tokens))
                {
                    log.Debug($"Sending pn with body {key.NotificationPayload?.Body} to {tokens.Count} tokens");
                    return send(tokens, key);
                }
            }
            return true;
        }

        public void Dispose()
        {
            log.Debug("Disposing the aggregator");
            if (timer != null)
            {
                lock (this)
                {
                    Flush();
                    timer?.Dispose();
                    timer = null;
                    canSend = false;
                }
            }
        }
    }
}
