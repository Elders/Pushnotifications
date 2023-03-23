//using System;
//using Elders.Cronus;
//using PushNotifications.Contracts;
//using PushNotifications.Contracts.Subscriptions.Events;
//using PushNotifications.Subscriptions.Events;

//namespace PushNotifications.Ports
//{
//    public class TopicSubscriptionPortTracker : IPort,
//      IEventHandler<SubscribedToTopic>,
//      IEventHandler<UnsubscribedFromTopic>
//    {
//        /// <summary>
//        /// alex: bright idea to make it mandatory
//        /// mynkow: It gets brighter when you use IGateway
//        /// </summary>
//        [Obsolete("Be brighter", true)]
//        public IPublisher<ICommand> CommandPublisher { get; set; }

//        public ITopicSubscriptionTrackerFactory StatsTrackerFactory { get; set; }

//        public void Handle(SubscribedToTopic @event)
//        {
//            StatsTrackerFactory.GetService(@event.Id.Tenant).Increment(@event.Id.Topic);
//        }

//        public void Handle(UnsubscribedFromTopic @event)
//        {
//            StatsTrackerFactory.GetService(@event.Id.Tenant).Decrement(@event.Id.Topic);
//        }
//    }
//}
