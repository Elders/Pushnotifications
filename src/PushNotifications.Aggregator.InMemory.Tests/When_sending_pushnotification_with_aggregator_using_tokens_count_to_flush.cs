using Machine.Specifications;
using System;
using System.Linq;
using PushNotifications.Contracts.PushNotifications.Delivery;
using PushNotifications.Contracts;
using PushNotifications.Contracts.PushNotifications;

namespace PushNotifications.Aggregator.InMemory.Tests
{
    [Subject(typeof(InMemoryPushNotificationAggregator))]
    public class When_sending_pushnotification_with_aggregator_using_tokens_count_to_flush
    {
        Establish ctx = () =>
        {
            timeSpanBeforeFlush = TimeSpan.FromDays(1);
            countOfRecipientsBeforeFlush = 5;
            concreateDelivery = new TestDeliveryWithInMemoryAggregator(timeSpanBeforeFlush, countOfRecipientsBeforeFlush);

            expirationDateOfNotification = Timestamp.JudgementDay();
            countOfRecipients = 10;
            notification = new NotificationForDelivery(new PushNotificationId(Guid.NewGuid().ToString(), "elders"), new NotificationPayload("title", "body"), expirationDateOfNotification, true);
        };

        Because of = () =>
        {
            Helper.Send(concreateDelivery, countOfRecipients, notification);
        };

        It should_have_sent_notifications_to_all_recipients = () => concreateDelivery.Store.Count().ShouldEqual(countOfRecipients);

        static TestDeliveryWithInMemoryAggregator concreateDelivery;
        static TimeSpan timeSpanBeforeFlush;
        static int countOfRecipientsBeforeFlush;
        static Timestamp expirationDateOfNotification;
        static int countOfRecipients;
        static NotificationForDelivery notification;
    }
}
