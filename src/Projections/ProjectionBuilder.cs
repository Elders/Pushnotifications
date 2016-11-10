using Elders.Cronus.DomainModeling;
using Elders.Cronus.DomainModeling.Projections;
using System;
using System.Collections.Generic;

namespace Projections
{
    public class ProjectionBuilder
    {
        public TProjection Rebuild<TProjection>(List<IEvent> events, IProjectionState snapshot)
        {
            if (ReferenceEquals(events, null) == true) throw new ArgumentNullException(nameof(events));

            TProjection proj = Activator.CreateInstance<TProjection>();

            dynamic gg = (dynamic)proj;

            if (ReferenceEquals(null, snapshot) == false)
                gg.State = (dynamic)snapshot;

            foreach (var @event in events)
            {
                gg.Handle((dynamic)@event);
            }

            if (ReferenceEquals(null, snapshot) && events.Count == 0)
                return default(TProjection);

            return proj;
        }
    }
}