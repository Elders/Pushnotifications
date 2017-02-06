using System;
using PushSharp.Core;
using PushNotifications.Pushy.Logging;

namespace PushNotifications.Pushy
{
    public class PushyNotificationService : IPushService
    {
        static ILog log = LogProvider.GetLogger(typeof(PushyNotificationService));
        private readonly string apiKey;
        const string PushyUrl = "https://api.pushy.me/push?api_key=";

        public PushyNotificationService(string apiKey)
        {
            this.apiKey = apiKey;
        }

        public void QueueNotification(INotification notification)
        {
            if (!(notification is PushyNotificationMessage))
                throw new Exception("Push broker is not working as expected");
            var not = (notification as PushyNotificationMessage).ToNotification();

            var client = new RestSharp.RestClient(PushyUrl + apiKey);
            var request = new RestSharp.RestRequest(RestSharp.Method.POST);
            request.RequestFormat = RestSharp.DataFormat.Json;
            request.AddJsonBody(not);

            string requestString = string.Empty;
            foreach (var item in request.Parameters)
            {
                requestString += Environment.NewLine + item.Name + ":" + item.Value;
            }
            try
            {
                var response = client.Execute(request);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    log.Error("Failure: PushyNotification -> " + requestString + " -> " + response.StatusCode.ToString() + " " + response.Content);
                else
                    log.Info("Sent Pushy Android Notification: " + requestString);
            }
            catch (Exception ex)
            {
                log.ErrorException("Failure: PushyNotification -> " + requestString + " -> ", ex);
            }
        }

        #region Unnescsairy
        public IPushChannelSettings ChannelSettings { get { return new NoChannelSettings(); } }

        public class NoChannelSettings : IPushChannelSettings { }
        public bool IsStopping { get { return false; } }

        public event ChannelCreatedDelegate OnChannelCreated;

        public event ChannelDestroyedDelegate OnChannelDestroyed;

        public event ChannelExceptionDelegate OnChannelException;

        public event DeviceSubscriptionChangedDelegate OnDeviceSubscriptionChanged;

        public event DeviceSubscriptionExpiredDelegate OnDeviceSubscriptionExpired;

        public event NotificationFailedDelegate OnNotificationFailed;

        public event NotificationRequeueDelegate OnNotificationRequeue;

        public event NotificationSentDelegate OnNotificationSent;

        public event ServiceExceptionDelegate OnServiceException;

        public IPushChannelFactory PushChannelFactory { get { return new NoChannelFactory(); } }

        public class NoChannelFactory : IPushChannelFactory
        {
            public IPushChannel CreateChannel(IPushChannelSettings channelSettings)
            {
                return new NoChannel();
            }

            public class NoChannel : IPushChannel
            {

                public void SendNotification(INotification notification, SendNotificationCallbackDelegate callback)
                {
                }

                public void Dispose()
                {
                }
            }
        }

        public IPushServiceSettings ServiceSettings
        {
            get { return new NoPushServiceSettings() { Channels = 1, MaxAutoScaleChannels = 2 }; }
        }

        public class NoPushServiceSettings : IPushServiceSettings
        {
            public NoPushServiceSettings()
            {
                AutoScaleChannels = true;
                Channels = 2;
                IdleTimeout = TimeSpan.Zero;
                MaxAutoScaleChannels = 2;
                MaxNotificationRequeues = 2;
                MinAvgTimeToScaleChannels = 500L;
                NotificationSendTimeout = 5000;
            }
            public bool AutoScaleChannels { get; set; }

            public int Channels { get; set; }

            public TimeSpan IdleTimeout { get; set; }

            public int MaxAutoScaleChannels { get; set; }

            public int MaxNotificationRequeues { get; set; }

            public long MinAvgTimeToScaleChannels { get; set; }

            public int NotificationSendTimeout { get; set; }
        }

        public void Stop(bool waitForQueueToFinish = true)
        {

        }

        public void Dispose()
        {

        }
        #endregion
    }
}
