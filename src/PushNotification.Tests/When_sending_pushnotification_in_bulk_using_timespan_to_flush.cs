using Machine.Specifications;
using System;
using System.Linq;
using PushNotifications.Contracts.PushNotifications.Delivery;
using PushNotifications.Contracts;
using PushNotifications.Delivery.Bulk;
using PushNotifications.Contracts.PushNotifications;
using System.Threading;

namespace PushNotification.Tests
{
    [Subject(typeof(InMemoryBufferedDelivery<TestDelivery>))]
    public class When_sending_pushnotification_in_bulk_using_timespan_to_flush
    {
        Establish ctx = () =>
        {
            timeSpanBeforeFlush = new TimeSpan(0, 0, 0, 1);
            countOfRecipientsBeforeFlus = int.MaxValue;
            concreateDelivery = new TestDelivery();
            bulkDelivery = new InMemoryBufferedDelivery<IPushNotificationBulkDelivery>(concreateDelivery, timeSpanBeforeFlush, countOfRecipientsBeforeFlus);

            expirationDateOfNotification = Timestamp.JudgementDay();
            countOfRecipients = 10000;
            notification = new TestNotificationDelivery(new PushNotificationId(Guid.NewGuid().ToString(), "elders"), new NotificationPayload("title", "body"), expirationDateOfNotification, true);
        };

        Because of = () =>
        {
            new Helper().Send(bulkDelivery, countOfRecipients, notification);
            Thread.Sleep(timeSpanBeforeFlush);
        };

        It should_have_sent_notifications_to_all_recipients_after_waiting_for_the_timespan = () => concreateDelivery.Store.Count().ShouldEqual(countOfRecipients);

        static TestDelivery concreateDelivery;
        static InMemoryBufferedDelivery<IPushNotificationBulkDelivery> bulkDelivery;
        static TimeSpan timeSpanBeforeFlush;
        static int countOfRecipientsBeforeFlus;
        static Timestamp expirationDateOfNotification;
        static int countOfRecipients;
        static NotificationDeliveryModel notification;
    }
}
