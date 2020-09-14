using Elders.Cronus;
using Elders.Cronus.Projections;
using PushNotifications.Contracts.PushNotifications.Delivery;
using PushNotifications.Projections.Subscriptions;
using PushNotifications.Subscriptions;
using PushNotifications.Subscriptions.Commands;

namespace PushNotifications.Ports
{
    public class PushNotificationTrigger : ITrigger,
        ISignalHandle<NotificationMessageSignal>
    {
        private readonly IProjectionReader projections;
        private readonly IPublisher<ICommand> publisher;
        private readonly MultiPlatformDelivery delivery;

        public PushNotificationTrigger(IProjectionReader projections, IPublisher<ICommand> publisher, MultiPlatformDelivery delivery)
        {
            this.projections = projections;
            this.publisher = publisher;
            this.delivery = delivery;
        }

        public void Handle(NotificationMessageSignal signal)
        {
            var subscriberId = new SubscriberId(AggregateUrn.Parse(signal.Recipient));
            var projectionResult = projections.Get<SubscriberTokensProjection>(subscriberId);
            if (projectionResult.IsSuccess)
            {
                var pushResult = delivery.SendAsync(projectionResult.Data.State.Tokens, signal.ToDelivery()).GetAwaiter().GetResult();
                if (pushResult.HasFailedTokens)
                {
                    foreach (var failedToken in pushResult.FailedTokens)
                    {
                        var subscribtionId = SubscriptionId.New(signal.Tenant, failedToken.Token);
                        var unsubscribe = new UnSubscribe(subscribtionId, subscriberId, failedToken);
                        publisher.Publish(unsubscribe);
                    }
                }
            }
        }

        //public void Send(NotificationMessage notification, )
        //{
        //    //var projectionResult = projections.Get<SubscriberTokensProjection>(null);
        //    //if (projectionResult.IsSuccess == false)
        //    //{
        //    //    // log.Info(() => $"No tokens were found for subscriber {@event.SubscriberId}");
        //    //    return;
        //    //}

        //    //foreach (var token in projectionResult.Data.State.Tokens)
        //    //{
        //    //    var delivery = deliveryProvisioner.ResolveDelivery(token.SubscriptionType, notification);
        //    //    SendTokensResult sendResult = delivery.Send(token, notification);

        //    //    if (sendResult.HasFailedTokens)
        //    //    {
        //    //        foreach (var failedToken in sendResult.FailedTokens)
        //    //        {
        //    //            var subscribtionId = SubscriptionId.New(@event.Id.Tenant, failedToken.Token);
        //    //            var unsubscribe = new UnSubscribe(subscribtionId, @event.SubscriberId, failedToken);
        //    //            publisher.Publish(unsubscribe);
        //    //        }
        //    //    }
        //    //}
        //}
    }

    //public class PushNotificationsPort : IPort,
    //    IEventHandler<PushNotificationSent>,
    //    IEventHandler<TopicPushNotificationSent>
    //{
    //    private readonly IProjectionReader projections;
    //    private readonly IDeliveryProvisioner deliveryProvisioner;

    //    public PushNotificationsPort(IPublisher<ICommand> commandPublisher, IProjectionReader projections, IDeliveryProvisioner deliveryProvisioner)
    //    {
    //        CommandPublisher = commandPublisher;
    //        this.projections = projections;
    //        this.deliveryProvisioner = deliveryProvisioner;
    //    }

    //    public IPublisher<ICommand> CommandPublisher { get; set; }

    //    public void Handle(PushNotificationSent @event)
    //    {
    //        var projectionReponse = projections.Get<SubscriberTokensProjection>(@event.SubscriberId);
    //        if (projectionReponse.IsSuccess == false)
    //        {
    //            // log.Info(() => $"No tokens were found for subscriber {@event.SubscriberId}");
    //            return;
    //        }

    //        foreach (var token in projectionReponse.Data.State.Tokens)
    //        {
    //            var notification = new NotificationForDelivery(@event.Id, @event.NotificationPayload, @event.NotificationData, @event.ExpiresAt, @event.ContentAvailable);
    //            var delivery = deliveryProvisioner.ResolveDelivery(token.SubscriptionType, notification);
    //            SendTokensResult sendResult = delivery.Send(token, notification);

    //            if (sendResult.HasFailedTokens)
    //            {
    //                foreach (var failedToken in sendResult.FailedTokens)
    //                {
    //                    var subscribtionId = SubscriptionId.New(@event.Id.Tenant, failedToken.Token);
    //                    var unsubscribe = new UnSubscribe(subscribtionId, @event.SubscriberId, failedToken);
    //                    if (CommandPublisher.Publish(unsubscribe) == false)
    //                    {
    //                        //log.Error("Unable to publish command" + Environment.NewLine + unsubscribe.ToString());
    //                    }
    //                }
    //            }
    //        }
    //    }

    //    public void Handle(TopicPushNotificationSent @event)
    //    {
    //        var topic = @event.Id.Topic;

    //        var notification = new NotificationForDelivery(@event.Id, @event.NotificationPayload, @event.NotificationData, @event.ExpiresAt, @event.ContentAvailable);

    //        var provisioners = deliveryProvisioner.GetDeliveryProviders(@event.Id.Tenant);

    //        foreach (var provisioner in provisioners)
    //        {
    //            provisioner.SendToTopic(topic, notification);
    //        }
    //    }
    //}
}
