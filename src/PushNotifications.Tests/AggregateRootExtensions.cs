using Elders.Cronus;

namespace PushNotifications.Tests
{
    public static class AggregateRootExtensions
    {
        public static T RootState<T>(this AggregateRoot<T> root)
            where T : IAggregateRootState, new()
        {
            return (T)(root as IHaveState<IAggregateRootState>).State;
        }
    }
}
