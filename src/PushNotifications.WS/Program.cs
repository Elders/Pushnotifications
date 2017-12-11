namespace PushNotifications.WS
{
    static class Program
    {
        static void Main()
        {
#if DEBUG
            var service = new PushNotificationService();
            service.Debug();
#else
            System.ServiceProcess.ServiceBase[] ServicesToRun;
            ServicesToRun = new System.ServiceProcess.ServiceBase[] { new PushNotificationService() };
            System.ServiceProcess.ServiceBase.Run(ServicesToRun);
#endif
        }
    }
}
