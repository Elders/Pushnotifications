using System;
using Elders.Cronus.DomainModeling;
using PushNotifications.Contracts;

namespace PushNotifications.Api.Client
{
    static class Usage
    {
        static void Main()
        {
            Uri authority = new Uri("https://account-vapt.local.com");
            Uri apiAddress = new Uri("https://pn-new.local.com");

            Uri qoreAuthority = new Uri("https://account-qore.local.com");

            var pnClient = new PushNotificationsRestClient(authority, "connect/token", "localclient", "localsecret", "read write admin", apiAddress);
            var pnClientForMarketVision = new PushNotificationsRestClient(qoreAuthority, "connect/token", "localclient", "localsecret", "read write admin", apiAddress);

            var t = Timestamp.JudgementDay();
            //for (int i = 0; i < 10; i++)
            //{
            //    var sent = pnClient.SendPushNotification(new Models.PushNotificationSendModel("pruvit", new Contracts.SubscriberId(i.ToString(), "pruvit"), $"titleee", "bodyy", "default", string.Empty, 0, t, true));

            //    //var x = pnClient.SubscribeForFireBase(new SubscribeForFireBaseModel("pruvit", new SubscriberId(i.ToString(), "pruvit").Urn.Value, new SubscriptionToken($"token-{i}")));
            //}

            var pruvitSubscriber = new Contracts.SubscriberId("76277", "pruvit");
            var pruvitUrn = StringTenantUrn.Parse(pruvitSubscriber.Urn.Value);
            var pruvitSubscribe = pnClient.SubscribeForFireBase(new SubscribeForFireBaseModel(pruvitUrn, new SubscriptionToken("exk_tnns_OI:APA91bFzlzvFysNlYpVmvdBbT1gVS66PeJ8izHLGwO0i3nu8TZGYBQrorURNM_fu2tPupGi9Zm1k-u1wIjwwclqdz-9IGjxbtqLa4PlH2B8h6T4xr1JMULKJbVeaZEcGJO1K2_0tmBxd")));
            var x = pnClient.SendPushNotification(new Models.PushNotificationSendModel(pruvitUrn, $"titleee", "bodyy", "default", string.Empty, 0, t, true));


            var mvSubscriber = new Contracts.SubscriberId("76277", "mv");
            var mvUrn = StringTenantUrn.Parse(mvSubscriber.Urn.Value);
            var s = pnClient.SubscribeForFireBase(new SubscribeForFireBaseModel(mvUrn, new SubscriptionToken("exk_tnns_OI:APA91bFzlzvFysNlYpVmvdBbT1gVS66PeJ8izHLGwO0i3nu8TZGYBQrorURNM_fu2tPupGi9Zm1k-u1wIjwwclqdz-9IGjxbtqLa4PlH2B8h6T4xr1JMULKJbVeaZEcGJO1K2_0tmBxd")));
            var xx = pnClientForMarketVision.SendPushNotification(new Models.PushNotificationSendModel( mvUrn, $"titleee - MV", "bodyy", "default", string.Empty, 0, t, true));
        }
    }
}
