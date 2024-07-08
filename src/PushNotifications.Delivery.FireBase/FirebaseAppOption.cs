using FirebaseAdmin;

namespace PushNotifications.Delivery.FireBase
{
    public class FirebaseAppOption
    {
        public FirebaseAppOption(string tenant, string application, AppOptions options)
        {
            Tenant = tenant;
            Application = application;
            Options = options;
        }

        public string Tenant { get; set; }
        public string Application { get; set; }
        public AppOptions Options { get; set; }
    }
}
