using Elders.Cronus;
using System.Runtime.Serialization;

namespace PushNotifications.Contracts
{
    [DataContract(Name = "ece0a261-027e-479b-9ce3-530d1acd8dfe")]
    public class SubscriberId : StringTenantId
    {
        SubscriberId() { }

        public SubscriberId(IUrn urn) : base(urn, "subscriber") { }

        public SubscriberId(string id, string tenant) : base(id, "subscriber", tenant) { }
    }
}
