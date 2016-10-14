using System;
using System.Collections.Generic;
using PushSharp.Core;

namespace PushNotifications.WS.NotificationThrottle
{
    public class ThrottledBrokerAdapter : IPushBroker
    {
        private ThrottledBroker actualBroker;
        public ThrottledBrokerAdapter(ThrottledBroker actualBroker)
        {
            this.actualBroker = actualBroker;
        }
        public IEnumerable<IPushService> GetAllRegistrations()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IPushService> GetRegistrations<TNotification>(string applicationId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IPushService> GetRegistrations(string applicationId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IPushService> GetRegistrations<TNotification>()
        {
            throw new NotImplementedException();
        }

        public event ChannelCreatedDelegate OnChannelCreated;

        public event ChannelDestroyedDelegate OnChannelDestroyed;

        public event ChannelExceptionDelegate OnChannelException;

        public event DeviceSubscriptionChangedDelegate OnDeviceSubscriptionChanged;

        public event DeviceSubscriptionExpiredDelegate OnDeviceSubscriptionExpired;

        public event NotificationFailedDelegate OnNotificationFailed;

        public event NotificationRequeueDelegate OnNotificationRequeue;

        public event NotificationSentDelegate OnNotificationSent;

        public event ServiceExceptionDelegate OnServiceException;

        public void QueueNotification<TPushNotification>(TPushNotification notification, string applicationId) where TPushNotification : Notification
        {
            throw new NotImplementedException();
        }

        public void QueueNotification<TPushNotification>(TPushNotification notification) where TPushNotification : Notification
        {
            actualBroker.QueueNotification(notification);
        }

        public void RegisterService<TPushNotification>(IPushService pushService, bool raiseErrorOnDuplicateRegistrations = true) where TPushNotification : Notification
        {
            throw new NotImplementedException();
        }

        public void RegisterService<TPushNotification>(IPushService pushService, string applicationId, bool raiseErrorOnDuplicateRegistrations = true) where TPushNotification : Notification
        {
            throw new NotImplementedException();
        }

        public void StopAllServices(string applicationId, bool waitForQueuesToFinish = true)
        {
            throw new NotImplementedException();
        }

        public void StopAllServices<TNotification>(string applicationId, bool waitForQueuesToFinish = true)
        {
            throw new NotImplementedException();
        }

        public void StopAllServices<TNotification>(bool waitForQueuesToFinish = true)
        {
            throw new NotImplementedException();
        }

        public void StopAllServices(bool waitForQueuesToFinish = true)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            if (actualBroker != null)
            {
                actualBroker.Dispose();
                actualBroker = null;
            }
        }
    }
}
