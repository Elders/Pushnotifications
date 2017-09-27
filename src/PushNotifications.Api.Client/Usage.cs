using System;

namespace PushNotifications.Api.Client
{
    static class Usage
    {
        static void Main()
        {

            Uri clientApiAddress = new Uri("https://pn.local.com");
            var clientApiClientOptions = new PushNotificationsClient.Options(clientApiAddress);
            var client = new PushNotificationsClient(clientApiClientOptions);

            Uri authority = new Uri("https://account-vapt.local.com");
            var authenticatorClientOptions = new Authenticator.Options()
            {
                Authority = authority,
                ClientId = "localclient",
                ClientSecret = "localsecret",
                Scope = "read write admin"
            };
            var authenticatorClient = new Authenticator(authenticatorClientOptions);
            Authenticator authenticator = authenticatorClient.GetClientCredentialsAuthenticatorAsync().Result;



            var model = new PushNotificationsClient.PushNotificationModel()
            {
                UserId = "1",
                Text = "text",
                Json = "{}",
                Badge = 0
            };
            var sent = client.Send(model, authenticator).Result;

        }
    }
}
