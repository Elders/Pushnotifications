﻿//using Machine.Specifications;
//using System;
//using System.Linq;
//using PushNotifications.Contracts.PushNotifications.Delivery;
//using PushNotifications.Contracts.PushNotifications;
//using System.Collections.Generic;

//namespace PushNotifications.Aggregator.InMemory.Tests
//{
//    [Subject(typeof(InMemoryPushNotificationAggregator))]
//    public class When_sending_pushnotification_with_aggregator_using_timespan_to_flush_and_does_not_wait
//    {
//        Establish ctx = () =>
//        {
//            timeSpanBeforeFlush = TimeSpan.FromSeconds(10);
//            countOfRecipientsBeforeFlush = int.MaxValue;
//            concreateDelivery = new TestDeliveryWithInMemoryAggregator(timeSpanBeforeFlush, countOfRecipientsBeforeFlush);

//            expirationDateOfNotification = DateTimeOffset.MaxValue;
//            countOfRecipients = 10;
//            var notificationData = new Dictionary<string, object>();
//            notificationData.Add("test", "test");
//            notification = new NotificationForDelivery(PushNotificationId.New("elders"), new NotificationPayload("title", "body"), notificationData, expirationDateOfNotification, true);
//        };

//        Because of = () => Helper.Send(concreateDelivery, countOfRecipients, notification);

//        It should_have_sent_zero_notifications = () => concreateDelivery.Store.Count().ShouldEqual(0);

//        static TestDeliveryWithInMemoryAggregator concreateDelivery;
//        static TimeSpan timeSpanBeforeFlush;
//        static int countOfRecipientsBeforeFlush;
//        static DateTimeOffset expirationDateOfNotification;
//        static int countOfRecipients;
//        static NotificationForDelivery notification;
//    }
//}
