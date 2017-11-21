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
    public class When_sending_one_pushnotification_to_mutiple_tokens_with_buffer
    {
        Establish ctx = () =>
        {
            timeSpanBeforeFlush = TimeSpan.FromSeconds(1);
            countOfRecipientsBeforeFlush = int.MaxValue;
            concreateDelivery = new TestDelivery();
            bufferedDelivery = new InMemoryBufferedDelivery<IPushNotificationBufferedDelivery>(concreateDelivery, timeSpanBeforeFlush, countOfRecipientsBeforeFlush);

            expirationDateOfNotification = Timestamp.JudgementDay();

            t1 = new SubscriptionToken("t1");
            t2 = new SubscriptionToken("t2");
            n1 = new NotificationForDelivery(new PushNotificationId("n1", "elders"), new NotificationPayload("title-1", "body"), expirationDateOfNotification, true);
        };

        Because of = () =>
        {
            bufferedDelivery.Send(t1, n1);
            bufferedDelivery.Send(t2, n1);
            Thread.Sleep((int)timeSpanBeforeFlush.TotalMilliseconds * 2);
        };

        It should_send_correct_number_of_notifications = () => concreateDelivery.Store.Count().ShouldEqual(2);


        It should_send_to_correct_first_token = () => concreateDelivery.Store.Where(x => x.Key.Equals(t1)).Count().ShouldEqual(1);

        It should_send_to_correct_second_token = () => concreateDelivery.Store.Where(x => x.Key.Equals(t2)).Count().ShouldEqual(1);

        It should_send_to_correct_notifications = () => { concreateDelivery.Store.First().Value.ShouldEqual(n1); };

        static TestDelivery concreateDelivery;
        static InMemoryBufferedDelivery<IPushNotificationBufferedDelivery> bufferedDelivery;
        static TimeSpan timeSpanBeforeFlush;
        static int countOfRecipientsBeforeFlush;
        static Timestamp expirationDateOfNotification;

        static SubscriptionToken t1;
        static SubscriptionToken t2;
        static NotificationForDelivery n1;
    }
}
