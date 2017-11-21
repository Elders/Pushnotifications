using Machine.Specifications;
using System;
using System.Linq;
using PushNotifications.Contracts.PushNotifications.Delivery;
using PushNotifications.Contracts;
using PushNotifications.Contracts.PushNotifications;
using System.Threading;
using PushNotifications.Delivery.Buffered;

namespace PushNotification.Tests
{
    [Subject(typeof(InMemoryBufferedDelivery<TestDelivery>))]
    public class When_sending_pushnotification_with_buffer_using_timespan_to_flush
    {
        Establish ctx = () =>
        {
            timeSpanBeforeFlush = TimeSpan.FromSeconds(1);
            countOfRecipientsBeforeFlus = int.MaxValue;
            concreateDelivery = new TestDelivery();
            bufferedDelivery = new InMemoryBufferedDelivery<IPushNotificationBufferedDelivery>(concreateDelivery, timeSpanBeforeFlush, countOfRecipientsBeforeFlus);

            expirationDateOfNotification = Timestamp.JudgementDay();
            countOfRecipients = 100000;
            notification = new NotificationForDelivery(new PushNotificationId(Guid.NewGuid().ToString(), "elders"), new NotificationPayload("title", "body"), expirationDateOfNotification, true);
        };

        Because of = () =>
        {
            Helper.Send(bufferedDelivery, countOfRecipients, notification);
            Thread.Sleep((int)timeSpanBeforeFlush.TotalMilliseconds * 3);
        };

        It should_have_sent_notifications_to_all_recipients_after_waiting_for_the_timespan = () => concreateDelivery.Store.Count().ShouldEqual(countOfRecipients);

        static TestDelivery concreateDelivery;
        static InMemoryBufferedDelivery<IPushNotificationBufferedDelivery> bufferedDelivery;
        static TimeSpan timeSpanBeforeFlush;
        static int countOfRecipientsBeforeFlus;
        static Timestamp expirationDateOfNotification;
        static int countOfRecipients;
        static NotificationForDelivery notification;
    }
}
