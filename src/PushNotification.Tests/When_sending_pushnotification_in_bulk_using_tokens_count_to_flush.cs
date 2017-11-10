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
    [Subject(typeof(BulkDelivery<TestDelivery>))]
    public class When_sending_pushnotification_in_bulk_using_tokens_count_to_flush
    {
        Establish ctx = () =>
        {
            timeSpanBeforeFlush = new TimeSpan(1, 0, 0); // 1 day
            countOfRecipientsBeforeFlus = 5;
            concreateDelivery = new TestDelivery();
            bulkDelivery = new BulkDelivery<IPushNotificationBulkDeliver>(concreateDelivery, timeSpanBeforeFlush, countOfRecipientsBeforeFlus);

            expirationDateOfNotification = Timestamp.JudgementDay();
            countOfRecipients = 10;
            notification = new NotificationDelivery(new NotificationPayload("title", "body"), expirationDateOfNotification, true);
        };

        Because of = () =>
        {
            new Helper().Send(bulkDelivery, countOfRecipients, notification);
        };

        It should_have_sent_notifications_to_all_recipients = () => concreateDelivery.Store.Count().ShouldEqual(countOfRecipients);

        static TestDelivery concreateDelivery;
        static BulkDelivery<IPushNotificationBulkDeliver> bulkDelivery;
        static TimeSpan timeSpanBeforeFlush;
        static int countOfRecipientsBeforeFlus;
        static Timestamp expirationDateOfNotification;
        static int countOfRecipients;
        static NotificationDelivery notification;
    }
}
