using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PushNotifications.Contracts;
using PushNotifications.Contracts.PushNotifications.Delivery;

namespace PushNotifications.Delivery.Bulk
{
    public class BulkDelivery<T> : IDisposable, IPushNotificationDelivery where T : IPushNotificationBulkDelivery
    {
        readonly T delivery;

        readonly TimeSpan timeSpanBeforeFlush;

        readonly int recipientsCountBeforeFlush;

        private Timer timer;

        ConcurrentDictionary<NotificationDeliveryModel, List<SubscriptionToken>> store;

        public BulkDelivery(T delivery, TimeSpan timeSpanBeforeFlush, int recipientsCountBeforeFlush)
        {
            if (ReferenceEquals(null, delivery) == true) throw new ArgumentNullException(nameof(delivery));
            if (ReferenceEquals(null, timeSpanBeforeFlush) == true) throw new ArgumentNullException(nameof(timeSpanBeforeFlush));

            this.delivery = delivery;
            this.timeSpanBeforeFlush = timeSpanBeforeFlush;
            this.recipientsCountBeforeFlush = recipientsCountBeforeFlush;

            store = new ConcurrentDictionary<NotificationDeliveryModel, List<SubscriptionToken>>();
            timer = new Timer(x => Flush(), this, TimeSpan.Zero, timeSpanBeforeFlush);
        }

        public void Send(SubscriptionToken token, NotificationDeliveryModel notification)
        {
            if (ReferenceEquals(null, token) == true) throw new ArgumentNullException(nameof(token));
            if (ReferenceEquals(null, notification) == true) throw new ArgumentNullException(nameof(notification));

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
        }

        public void Dispose()
        {
            if (timer != null)
            {
                lock (this)
                {
                    timer?.Dispose();
                    timer = null;
                }
            }
        }
    }
}
