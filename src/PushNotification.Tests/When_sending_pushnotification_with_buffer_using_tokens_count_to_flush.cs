using Machine.Specifications;
using System;
using System.Linq;
using PushNotifications.Contracts.PushNotifications.Delivery;
using PushNotifications.Contracts;
using PushNotifications.Contracts.PushNotifications;
using PushNotifications.Delivery.Buffered;

namespace PushNotification.Tests
{
    [Subject(typeof(InMemoryBufferedDelivery<TestDelivery>))]
    public class When_sending_pushnotification_with_buffer_using_tokens_count_to_flush
    {
        Establish ctx = () =>
        {
            timeSpanBeforeFlush = TimeSpan.FromDays(1);
            countOfRecipientsBeforeFlush = 5;
            concreateDelivery = new TestDelivery();
            bufferedDelivery = new InMemoryBufferedDelivery<IPushNotificationDeliveryCapableOfSendingMoreThenOneNotificationAtOnce>(concreateDelivery, timeSpanBeforeFlush, countOfRecipientsBeforeFlush);

            expirationDateOfNotification = Timestamp.JudgementDay();
            countOfRecipients = 10;
            notification = new NotificationForDelivery(new PushNotificationId(Guid.NewGuid().ToString(), "elders"), new NotificationPayload("title", "body"), expirationDateOfNotification, true);
        };

        Because of = () =>
        {
            Helper.Send(bufferedDelivery, countOfRecipients, notification);
        };

        It should_have_sent_notifications_to_all_recipients = () => concreateDelivery.Store.Count().ShouldEqual(countOfRecipients);

        static TestDelivery concreateDelivery;
        static InMemoryBufferedDelivery<IPushNotificationDeliveryCapableOfSendingMoreThenOneNotificationAtOnce> bufferedDelivery;
        static TimeSpan timeSpanBeforeFlush;
        static int countOfRecipientsBeforeFlush;
        static Timestamp expirationDateOfNotification;
        static int countOfRecipients;
        static NotificationForDelivery notification;
    }
}
