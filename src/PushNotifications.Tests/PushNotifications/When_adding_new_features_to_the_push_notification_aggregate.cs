using System.Reflection;
using Machine.Specifications;

namespace PushNotifications.Tests.PushNotifications
{
    [Subject(nameof(PushNotification))]
    public class When_adding_new_features_to_the_push_notification_aggregate
    {
        Establish context = () => expectedMethodsCount = 1;

        Because of = () =>
        {
            var methods = typeof(PushNotification).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Length;
            var constructors = typeof(PushNotification).GetConstructors(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance).Length;
            methodsCount = methods + constructors;
        };

        It should_have_new_test_for_them = () => methodsCount.ShouldBeLessThanOrEqualTo(expectedMethodsCount);

        static int methodsCount;
        static int expectedMethodsCount;
    }
}
