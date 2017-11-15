using Machine.Specifications;
using System;
using System.Linq;
using PushNotifications.Contracts.PushNotifications.Delivery;
using PushNotifications.Contracts;
using PushNotifications.Delivery.Bulk;
using PushNotifications.Contracts.PushNotifications;

namespace PushNotification.Tests
{
    [Subject(typeof(InMemoryBufferedDelivery<TestDelivery>))]
    public class When_sending_pushnotification_in_bulk_using_tokens_count_to_flush
    {
        Establish ctx = () =>
        {
            timeSpanBeforeFlush = new TimeSpan(1, 0, 0); // 1 day
            countOfRecipientsBeforeFlus = 5;
            concreateDelivery = new TestDelivery();
            bulkDelivery = new InMemoryBufferedDelivery<IPushNotificationBulkDelivery>(concreateDelivery, timeSpanBeforeFlush, countOfRecipientsBeforeFlus);

            expirationDateOfNotification = Timestamp.JudgementDay();
            countOfRecipients = 10;
            notification = new TestNotificationDelivery(new PushNotificationId(Guid.NewGuid().ToString(), "elders"), new NotificationPayload("title", "body"), expirationDateOfNotification, true);
        };

        Because of = () =>
        {
            new Helper().Send(bulkDelivery, countOfRecipients, notification);
        };

        It should_have_sent_notifications_to_all_recipients = () => concreateDelivery.Store.Count().ShouldEqual(countOfRecipients);

        static TestDelivery concreateDelivery;
        static InMemoryBufferedDelivery<IPushNotificationBulkDelivery> bulkDelivery;
        static TimeSpan timeSpanBeforeFlush;
        static int countOfRecipientsBeforeFlus;
        static Timestamp expirationDateOfNotification;
        static int countOfRecipients;
        static NotificationDeliveryModel notification;
    }
}
