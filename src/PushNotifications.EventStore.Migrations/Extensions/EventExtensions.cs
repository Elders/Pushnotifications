using System.Collections.Generic;
using System.Linq;
using System.Text;
using Elders.Cronus;

namespace Vapt.EventStore.Migrations.Extensions
{
    public static class EventExtensions
    {
        public static string GetPropertiesAsString(this IEvent @event, List<string> filter = null)
        {
            var type = @event.GetType();
            var properties = type.GetProperties();

            if (ReferenceEquals(null, filter) == false && filter.Count > 0)
                properties = properties.Where(x => filter.Select(y => y.ToLower()).Contains(x.Name.ToLower())).ToArray();

            StringBuilder sb = new StringBuilder();

            sb.AppendLine();
            sb.AppendLine($"Type = {type.Name}");
            sb.AppendLine();

            foreach (var property in properties)
            {
                sb.AppendLine($"{property.Name} value = {property.GetValue(@event, null)}");
            }

            return sb.ToString();
        }

        public static IEvent Unwrap(this IEvent @event)
        {
            var entityEventContractId = typeof(EntityEvent).GetContractId();
            var currentEventContractId = @event.GetType().GetContractId();
            var shouldUnwrap = currentEventContractId.Equals(entityEventContractId);

            var Event = shouldUnwrap ? ((EntityEvent)@event).Event : @event;

            return Event;
        }

        public static IEnumerable<MigrationEvent> Normalize(this IEnumerable<IEvent> raw)
        {
            foreach (var rawEvent in raw)
            {
                yield return new MigrationEvent(rawEvent);
            }
        }
    }
}
