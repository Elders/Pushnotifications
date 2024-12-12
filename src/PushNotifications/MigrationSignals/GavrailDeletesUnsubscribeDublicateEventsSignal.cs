using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Elders.Cronus;

namespace PushNotifications.MigrationSignals
{
    [DataContract(Name = "c22bdf97-c374-45f0-ab5f-5aa99545fde3", Namespace = "pushnotifications")]
    public class GavrailDeletesUnsubscribeDublicateEventsSignal : ISignal
    {
        public GavrailDeletesUnsubscribeDublicateEventsSignal() { }

        public GavrailDeletesUnsubscribeDublicateEventsSignal(string tenant, bool isDryRun, DateTimeOffset timestamp)
        {
            Tenant = tenant;
            IsDryRun = isDryRun;
            Timestamp = timestamp;
        }

        [DataMember(Order = 0)]
        public string Tenant { get; private set; }

        [DataMember(Order = 1)]
        public bool IsDryRun { get; private set; } = true;

        [DataMember(Order = 2)]
        public DateTimeOffset Timestamp { get; private set; }
    }
}
