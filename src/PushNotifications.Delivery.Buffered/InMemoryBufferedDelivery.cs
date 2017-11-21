using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using PushNotifications.Contracts;
using PushNotifications.Contracts.PushNotifications.Delivery;

namespace PushNotifications.Delivery.Buffered
{
    public class InMemoryBufferedDelivery<T> : IDisposable, IPushNotificationDelivery where T : IPushNotificationBufferedDelivery
    {
        readonly T delivery;

        readonly TimeSpan timeSpanBeforeFlush;

        readonly int recipientsCountBeforeFlush;

        private Timer timer;

        bool canSend;

        ConcurrentDictionary<NotificationForDelivery, List<SubscriptionToken>> buffer;

        public InMemoryBufferedDelivery(T delivery, TimeSpan timeSpanBeforeFlush, int recipientsCountBeforeFlush)
        {
            if (ReferenceEquals(null, delivery) == true) throw new ArgumentNullException(nameof(delivery));
            if (ReferenceEquals(null, timeSpanBeforeFlush) == true) throw new ArgumentNullException(nameof(timeSpanBeforeFlush));

            this.delivery = delivery;
            this.timeSpanBeforeFlush = timeSpanBeforeFlush;
            this.recipientsCountBeforeFlush = recipientsCountBeforeFlush;
            canSend = true;
            buffer = new ConcurrentDictionary<NotificationForDelivery, List<SubscriptionToken>>();
            timer = new Timer(x => Flush(), this, TimeSpan.FromSeconds(0), timeSpanBeforeFlush);
        }

        public bool Send(SubscriptionToken token, NotificationForDelivery notification)
        {
            if (ReferenceEquals(null, token) == true) throw new ArgumentNullException(nameof(token));
            if (ReferenceEquals(null, notification) == true) throw new ArgumentNullException(nameof(notification));
            if (canSend == false)
                return false;
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
                    return delivery.Send(tokens, key);
                }
            }
            return true;
        }

        public void Dispose()
        {
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
