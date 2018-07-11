using System.Reflection;
using Machine.Specifications;
using PushNotifications.Subscriptions;

namespace PushNotifications.Tests.Subscriptions
{
    [Subject(nameof(TopicSubscription))]
    public class When_adding_new_features_to_the_topic_subscription_aggregate
    {
        Establish context = () => expectedMethodsCount = 3;

        Because of = () =>
        {
            var methods = typeof(TopicSubscription).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Length;
            var constructors = typeof(TopicSubscription).GetConstructors(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance).Length;
            methodsCount = methods + constructors;
        };

        It should_have_new_test_for_them = () => methodsCount.ShouldBeLessThanOrEqualTo(expectedMethodsCount);

        static int methodsCount;
        static int expectedMethodsCount;
    }
}
