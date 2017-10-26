using Elders.Cronus.DomainModeling;

namespace PushNotifications.Contracts
{
    public class UserId : StringTenantId
    {
        UserId() { }

        public UserId(IUrn id) : base(id, "Profile") { }

        public UserId(string id, string tenant) : base(id, "Profile", tenant) { }
    }
}
