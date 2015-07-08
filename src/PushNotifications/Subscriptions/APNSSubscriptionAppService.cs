using Elders.Cronus.DomainModeling;
using PushNotifications.Contracts.Subscriptions.Commands;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PushNotifications.Subscriptions
{
    public class APNSSubscriptionAppService : AggregateRootApplicationService<APNSSubscription>,
        ICommandHandler<SubscribeForAPNS>,
        ICommandHandler<UnSubscribeFromAPNS>
    {
        public void Handle(SubscribeForAPNS command)
        {
            APNSSubscription sub;

            if (Repository.TryLoad(command.Id, out sub))
            {
                Update(command.Id, x => x.Subscribe(command.UserId, command.Token));
            }
            else
            {
                sub = new APNSSubscription(command.Id, command.UserId, command.Token);

                Repository.Save<APNSSubscription>(sub);
            }
        }

        public void Handle(UnSubscribeFromAPNS command)
        {
            Update(command.Id, x => x.UnSubscribe(command.UserId, command.Token));
        }
    }
}