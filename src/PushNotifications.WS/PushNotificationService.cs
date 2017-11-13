using System;
using System.Net;
using System.ServiceProcess;
using Elders.Pandora;

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
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            var appContext = new ApplicationContext("pushnotifications.ws");
            var cfgRepo = new ConsulForPandora(new Uri("http://consul.local.com:8500"));
            var pandora = new Pandora(appContext, cfgRepo);
            LogStartup.Boot(pandora);
            PushNotificationStartUp.Start(pandora);
        }

        protected override void OnStop()
        {
            PushNotificationStartUp.Stop();
        }
    }
}
