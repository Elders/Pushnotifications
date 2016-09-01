using System;
using System.Threading.Tasks;
using RestSharp;

namespace PushNotifications.Api.Client
{
    public sealed partial class PushNotificationsClient
    {
        public async Task<IRestResponse> Send(PushNotificationModel model, Authenticator authenticator = null)
        {
            const string Resource = "PushNotifications/Send";
            if (ReferenceEquals(null, model)) throw new ArgumentNullException(nameof(model));

            var request = CreateRestRequest(Resource, Method.POST, authenticator)
                .AddJsonBody(model);

            return await CreateRestClient().ExecuteAsync(request);
        }

        public class PushNotificationModel
        {
            public PushNotificationModel(string userId, string json, string text, string sound, string icon, string category, int badge, bool isSilent)
            {
                UserId = userId;
                Json = json;
                Text = text;
                Sound = sound;
                Icon = icon;
                Category = category;
                Badge = badge;
                IsSilent = isSilent;
            }

            public string UserId { get; set; }

            public string Json { get; set; }

            public string Text { get; set; }

            public string Sound { get; set; }

            public string Icon { get; set; }

            public string Category { get; set; }

            public int Badge { get; set; }

            public bool IsSilent { get; set; }
        }
    }
}
