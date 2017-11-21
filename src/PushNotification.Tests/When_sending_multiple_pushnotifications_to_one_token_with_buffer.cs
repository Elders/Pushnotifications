using Machine.Specifications;
using System;
using System.Linq;
using PushNotifications.Contracts.PushNotifications.Delivery;
using PushNotifications.Contracts;
using PushNotifications.Contracts.PushNotifications;
using PushNotifications.Delivery.Buffered;
using System.Threading;

namespace PushNotification.Tests
{
    [Subject(typeof(InMemoryBufferedDelivery<TestDelivery>))]
    public class When_sending_multiple_pushnotifications_to_one_token_with_buffer
    {
        Establish ctx = () =>
        {
            timeSpanBeforeFlush = TimeSpan.FromSeconds(1);
            countOfRecipientsBeforeFlush = int.MaxValue;
            concreateDelivery = new TestDelivery();
            bufferedDelivery = new InMemoryBufferedDelivery<IPushNotificationBufferedDelivery>(concreateDelivery, timeSpanBeforeFlush, countOfRecipientsBeforeFlush);

            expirationDateOfNotification = Timestamp.JudgementDay();

            t1 = new SubscriptionToken("t1");
            n1 = new NotificationForDelivery(new PushNotificationId("n1", "elders"), new NotificationPayload("title-1", "body"), expirationDateOfNotification, true);
            n2 = new NotificationForDelivery(new PushNotificationId("n2", "elders"), new NotificationPayload("title-2", "body"), expirationDateOfNotification, true);
        };

        Because of = () =>
        {
            bufferedDelivery.Send(t1, n1);
            bufferedDelivery.Send(t1, n2);
            Thread.Sleep((int)timeSpanBeforeFlush.TotalMilliseconds * 3);
        };

        It should_send_correct_number_of_notifications = () => concreateDelivery.Store.Count().ShouldEqual(2);

        It should_send_to_correct_number_of_tokens = () => { concreateDelivery.Store.Where(x => x.Key == t1).Count().ShouldEqual(2); };

        It should_send_to_correct_first_notification = () => concreateDelivery.Store.Where(x => x.Value.Equals(n1)).Count().ShouldEqual(1);

        It should_send_to_correct_second_notification = () => concreateDelivery.Store.Where(x => x.Value.Equals(n2)).Count().ShouldEqual(1);

        static TestDelivery concreateDelivery;
        static InMemoryBufferedDelivery<IPushNotificationBufferedDelivery> bufferedDelivery;
        static TimeSpan timeSpanBeforeFlush;
        static int countOfRecipientsBeforeFlush;
        static Timestamp expirationDateOfNotification;

        static SubscriptionToken t1;
        static NotificationForDelivery n1;
        static NotificationForDelivery n2;
    }
}
