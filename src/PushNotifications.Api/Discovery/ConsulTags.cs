using System;

namespace PushNotifications.Api.Discovery
{
    public static class ConsulHelper
    {
        public static readonly Uri ConsulUri = new Uri("http://consul.local.com:8500");

        public const string Visability = "Visability";
        public const string BoundedContext = "BoundedContext";
        public const string UpdatedAt = "UpdatedAt";
        public const string IntroducedAtVersion = "IntroducedAtVersion";
        public const string DepricatedAtVersion = "DepricatedAtVersion";
        public const string EndpointName = "EndpointName";
        public const string EndpointUrl = "EndpointUrl";
        public const string Separator = "__";

        public static bool IsPublic(string tag)
        {
            if (string.IsNullOrEmpty(tag) == true) return false;

            var splitted = tag.Split(new string[] { Separator }, StringSplitOptions.RemoveEmptyEntries);
            if (splitted.Length < 2) return false;
            if (splitted[0] != Visability) return false;
            if (splitted[1] == "public") return true;

            return false;
        }
    }
}
