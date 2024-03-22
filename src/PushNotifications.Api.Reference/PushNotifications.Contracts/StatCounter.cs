using System.Runtime.Serialization;
using Elders.Cronus;

namespace PushNotifications.Contracts
{
    [DataContract(Name = "cc22c415-2e0f-48dc-85bb-6c9509cec7c1")]
    public class StatCounter : ValueObject<StatCounter>
    {
        public StatCounter(string name, long count)
        {
            if (string.IsNullOrEmpty(name)) throw new System.ArgumentNullException(nameof(name));

            Name = name;
            Count = count;
        }

        [DataMember(Order = 1)]
        public string Name { get; private set; }

        [DataMember(Order = 2)]
        public long Count { get; private set; }

        public static StatCounter Empty(string name) => new StatCounter(name, 0);
    }
}
