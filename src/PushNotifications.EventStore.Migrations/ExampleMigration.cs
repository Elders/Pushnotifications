using System.Collections.Generic;
using Elders.Cronus.EventStore;
using Elders.Cronus.Migration.Middleware;

namespace Vapt.EventStore.Migrations
{
    public class ExampleMigration : IMigration<AggregateCommit, IEnumerable<AggregateCommit>>
    {
        Dictionary<byte[], int> revisionFactory;

        public ExampleMigration(Dictionary<byte[], int> revisionFactory)
        {
            this.revisionFactory = revisionFactory;
        }

        public IEnumerable<AggregateCommit> Apply(AggregateCommit current)
        {
            var rev = 0;
            if (revisionFactory.ContainsKey(current.AggregateRootId))
            {
                rev = revisionFactory[current.AggregateRootId];
                rev = rev++;
            }
            else
            {
                revisionFactory.Add(current.AggregateRootId, rev);
            }

            return new List<AggregateCommit>() { current };
        }

        public bool ShouldApply(AggregateCommit current)
        {
            return true;
        }
    }
}
