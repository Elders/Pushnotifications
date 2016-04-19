using System;
using PushSharp.Core;

namespace PushNotifications.Ports.Parse
{
    public class ParseNotificationService : IPushService
    {
        static log4net.ILog log = log4net.LogManager.GetLogger(typeof(ParseNotificationService));
        const string ApplicationIdHeader = "X-Parse-Application-Id";
        const string RestKeyHeader = "X-Parse-REST-API-Key";
        const string ContentTypeJson = "application/json";
        const string ParseUrl = "https://api.parse.com/1/push";

        private readonly string applicationId;
        private readonly string restkey;

        public ParseNotificationService(string applicationId, string restkey)
        {
            this.restkey = restkey;
            this.applicationId = applicationId;
        }

        public void QueueNotification(INotification notification)
        {
            if (!(notification is ParseAndroidNotifcation))
                throw new Exception("Push broker is not working as expected");
            var not = (notification as ParseAndroidNotifcation).BuildNotificationObject();

            var client = new RestSharp.RestClient(ParseUrl);
            var request = new RestSharp.RestRequest(RestSharp.Method.POST);
            request.AddHeader(ApplicationIdHeader, applicationId);
            request.AddHeader(RestKeyHeader, restkey);
            // request.AddHeader(ContentTypeJson, ContentTypeJson);
            request.RequestFormat = RestSharp.DataFormat.Json;
            request.AddJsonBody(not);

            string requestString = string.Empty;
            foreach (var item in request.Parameters)
            {
                requestString += Environment.NewLine + item.Name + ":" + item.Value.ToString();
            }
            try
            {
                var response = client.Execute(request);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    log.Error("Failure: ParseNotification -> " + requestString + " -> " + response.StatusCode.ToString() + " " + response.Content);
                else
                    log.Info("Sent Parse Notification: " + requestString);
            }
            catch (Exception ex)
            {
                log.Error("Failure: ParseNotification -> " + requestString + " -> ", ex);
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

        public IPushServiceSettings ServiceSettings
        {
            get { return new NoPushServiceSettings() { Channels = 1, MaxAutoScaleChannels = 2 }; }
        }

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