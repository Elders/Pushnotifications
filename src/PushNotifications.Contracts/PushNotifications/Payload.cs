using Elders.Cronus.DomainModeling;
using System.Runtime.Serialization;

namespace PushNotifications.Contracts.PushNotifications
{
    [DataContract(Name = "2d1f9865-5800-4408-aa99-cfa866242b9d")]
    public class Payload : ValueObject<Payload>
    {
        Payload() { }

        public Payload(string json, string text, string sound, string icon, int badge, bool isSilent)
        {
            Json = json;
            Text = text;
            Sound = sound;
            Icon = icon;
            Badge = badge;
            IsSilent = isSilent;
        }

        [DataMember(Order = 1)]
        public string Json { get; private set; }

        [DataMember(Order = 2)]
        public string Text { get; private set; }

        [DataMember(Order = 3)]
        public string Sound { get; private set; }

        [DataMember(Order = 4)]
        public string Icon { get; private set; }

        [DataMember(Order = 5)]
        public int Badge { get; private set; }

        [DataMember(Order = 6)]
        public bool IsSilent { get; private set; }
    }
}