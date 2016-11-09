﻿using System.ServiceProcess;

namespace PushNotifications.WS
{
    public partial class PushNotificationService : ServiceBase
    {
        public PushNotificationService()
        {
            InitializeComponent();
        }

        public void Debug()
        {
            OnStart(null);
        }

        protected override void OnStart(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();
            Cronus.Start();
        }

        protected override void OnStop()
        {
            Cronus.Stop();
        }
    }
}
