using Machine.Specifications;
using PushNotifications.Contracts.PushNotifications.Delivery;
using PushNotifications.Contracts;
using PushNotifications.Contracts.PushNotifications;
using PushNotifications.Delivery.Buffered;

namespace PushNotification.Tests
{
    [Subject(typeof(InMemoryBufferedDelivery<TestDelivery>))]
    public class When_comparing_notification_delivery_model
    {
        Establish ctx = () =>
        {
            var expirationDateOfNotification = Timestamp.UtcNow();
            n1 = new TestNotificationDelivery(new PushNotificationId("n1", "elders"), new NotificationPayload("title-1", "body"), expirationDateOfNotification, true);
            n11 = new TestNotificationDelivery(new PushNotificationId("n1", "elders"), new NotificationPayload("title-1", "body"), expirationDateOfNotification, true);
            n12 = new TestNotificationDelivery(new PushNotificationId("n2", "elders"), new NotificationPayload("title-2", "body"), expirationDateOfNotification, true);
            n2 = new AnotherTestNotificationDelivery(new PushNotificationId("n1", "elders"), new NotificationPayload("title-1", "body"), expirationDateOfNotification, true);
        };

        Because of = () => { };

        It should_equal_to_itself = () => n1.ShouldEqual(n1);

        It should_equal_to_new_instance_of_same_type_with_equal_data = () => n1.ShouldEqual(n11);

        It should_not_equal_to_new_instance_of_same_type_with_different_data = () => n1.ShouldEqual(n11);

        It should_not_equal_to_new_instance_of_different_type_with_equal_data = () => n1.ShouldNotEqual(n2);

        static NotificationDeliveryModel n1;
        static NotificationDeliveryModel n11;
        static NotificationDeliveryModel n12;
        static AnotherTestNotificationDelivery n2;
    }
}
