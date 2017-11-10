using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PushNotifications.Contracts;
using PushNotifications.Contracts.PushNotifications.Delivery;

namespace PushNotifications.Delivery.Bulk
{
    public class BulkDelivery<T> : IPushNotificationDeliver where T : IPushNotificationBulkDeliver
    {
        readonly T delivery;

        readonly TimeSpan timeSpanBeforeFlush;

        readonly int recipientsCountBeforeFlush;

        ConcurrentDictionary<NotificationDelivery, List<SubscriptionToken>> store;


        public BulkDelivery(T delivery, TimeSpan timeSpanBeforeFlush, int recipientsCountBeforeFlush)
        {
            if (ReferenceEquals(null, delivery) == true) throw new ArgumentNullException(nameof(delivery));
            if (ReferenceEquals(null, timeSpanBeforeFlush) == true) throw new ArgumentNullException(nameof(timeSpanBeforeFlush));

            this.delivery = delivery;
            this.timeSpanBeforeFlush = timeSpanBeforeFlush;
            this.recipientsCountBeforeFlush = recipientsCountBeforeFlush;

            store = new ConcurrentDictionary<NotificationDelivery, List<SubscriptionToken>>();
            Flush();
        }

        public void Send(SubscriptionToken token, NotificationDelivery notification)
        {
            store.AddOrUpdate(notification, new List<SubscriptionToken> { token }, (k, v) => { v.Add(token); return v; });

            List<SubscriptionToken> tokens;
            if (store.TryGetValue(notification, out tokens) && tokens.Count >= recipientsCountBeforeFlush)
                Flush();
        }

        void Flush()
        {
            foreach (var key in store.Keys)
            {
                List<SubscriptionToken> tokens;
                if (store.TryRemove(key, out tokens))
                {
                    delivery.Send(tokens, key);
                }
            }

            // (nextExecution < DateTime.UtcNow)
            {
                var nextExecution = DateTime.UtcNow.Add(timeSpanBeforeFlush);
                Task.Delay(timeSpanBeforeFlush).ContinueWith(t => Flush());
            }
        }
    }
}
