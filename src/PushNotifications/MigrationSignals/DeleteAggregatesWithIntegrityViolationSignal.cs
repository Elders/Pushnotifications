using Elders.Cronus;
using System;
using System.Runtime.Serialization;

namespace PushNotifications.MigrationSignals
{
    [DataContract(Namespace = "pushnotifications", Name = "49447170-7360-457e-b182-b316e7279532")]
    public sealed class DeleteAggregatesWithIntegrityViolationSignal : ISignal
    {
        DeleteAggregatesWithIntegrityViolationSignal()
        {
            Timestamp = DateTimeOffset.UtcNow;
        }

        public DeleteAggregatesWithIntegrityViolationSignal(string tenant, bool isDryRun) : this()
        {
            Tenant = tenant;
            IsDryRun = isDryRun;
        }

        public string Tenant { get; set; }

        public bool IsDryRun { get; set; }

        public DateTimeOffset Timestamp { get; set; }
    }
}
