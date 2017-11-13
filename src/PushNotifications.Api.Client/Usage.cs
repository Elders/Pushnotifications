using System;
using PushNotifications.Contracts;

namespace PushNotifications.Api.Client
{
    static class Usage
    {
        static void Main()
        {
            Uri authority = new Uri("https://account-vapt.local.com");
            Uri apiAddress = new Uri("https://pn-new.local.com");

            var pnClient = new PushNotificationsRestClient(authority, "connect/token", "localclient", "localsecret", "read write admin", apiAddress);

            var t = Timestamp.JudgementDay();
            for (int i = 0; i < 10; i++)
            {
                var sent = pnClient.SendPushNotification(new Models.PushNotificationSendModel("pruvit", new Contracts.SubscriberId(i.ToString(), "pruvit"), $"titleee", "bodyy", "default", string.Empty, 0, t, true));

                //var x = pnClient.SubscribeForFireBase(new SubscribeForFireBaseModel("pruvit", new SubscriberId(i.ToString(), "pruvit").Urn.Value, new SubscriptionToken($"token-{i}")));
            }

            var xx = pnClient.SendPushNotification(new Models.PushNotificationSendModel("pruvit", new Contracts.SubscriberId("76277", "pruvit"), $"titleee", "bodyy", "default", string.Empty, 0, t, true));
        }
    }
}
