using PushSharp.Core;
using System.Collections.Generic;

namespace PushNotifications.Pushy
{
    public class PushyNotification : Notification
    {
        private readonly List<string> tokens;

        public PushyNotification(string jsonData, List<string> tokens)
        {
            JsonnData = jsonData;
            this.tokens = tokens;
        }

        public string JsonnData { get; private set; }

        public IEnumerable<string> Tokens { get { return tokens.AsReadOnly(); } }
    }
}
