using System.Reflection;
using Machine.Specifications;
using PushNotifications.Subscriptions;

namespace PushNotifications.Tests.Subscriptions
{
    [Subject(nameof(Subscription))]
    public class When_adding_new_features_to_the_subscription_aggregate
    {
        Establish context = () => expectedMethodsCount = 3;

        Because of = () =>
        {
            var methods = typeof(Subscription).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Length;
            var constructors = typeof(Subscription).GetConstructors(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance).Length;
            methodsCount = methods + constructors;
        };

        It should_have_new_test_for_them = () => methodsCount.ShouldBeLessThanOrEqualTo(expectedMethodsCount);

        static int methodsCount;
        static int expectedMethodsCount;
    }
}
