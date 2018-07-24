using Cassandra;
using PushNotifications.Contracts;
using System.Collections.Generic;

namespace Multitenancy.Tracker
{
    public partial class TopicSubscriptionTracker : ITopicSubscriptionTracker
    {
        const string CreateTableTemplate = @"CREATE TABLE IF NOT EXISTS ""pushnot_subscriptions"" (cv counter, name varchar, PRIMARY KEY (name));";
        const string IncrementTemplate = @"UPDATE ""pushnot_subscriptions"" SET cv = cv + 1 WHERE name=?;";
        const string DecrementTemplate = @"UPDATE ""pushnot_subscriptions"" SET cv = cv - 1 WHERE name=?;";
        const string GetTemplate = @"SELECT * FROM pushnot_subscriptions WHERE name=?;";

        private readonly ISession _session;
        private readonly PreparedStatement incrementTemplate;
        private readonly PreparedStatement decrementTemplate;
        private readonly PreparedStatement getTemplate;

        public TopicSubscriptionTracker(ISession session)
        {
            _session = session;

            _session.Execute(CreateTableTemplate);

            incrementTemplate = session.Prepare(IncrementTemplate);
            decrementTemplate = session.Prepare(DecrementTemplate);
            getTemplate = session.Prepare(GetTemplate);
        }

        public void Increment(string name)
        {
            var stat = new TopicSubscriptionStat(name);
            IncrementTrack(stat);
        }

        public void Decrement(string name)
        {
            var stat = new TopicSubscriptionStat(name);
            DecrementTrack(stat);
        }

        private void IncrementTrack(TopicSubscriptionStat stat)
        {
            PreparedStatement query = incrementTemplate;
            _session.Execute(query.Bind(stat.Name));
        }

        private void DecrementTrack(TopicSubscriptionStat stat)
        {
            PreparedStatement query = decrementTemplate;
            _session.Execute(query.Bind(stat.Name));
        }

        public IEnumerable<StatCounter> Show(string name)
        {
            foreach (StatCounter statCounter in GetStatsTable(name))
            {
                yield return statCounter;
            }
        }

        private IEnumerable<StatCounter> GetStatsTable(string name)
        {
            PreparedStatement query = getTemplate;

            BoundStatement boundedStatement = query.Bind(name);
            RowSet result = _session.Execute(boundedStatement);

            foreach (Row row in result.GetRows())
            {
                long count = row.GetValue<long>("cv");
                var stat = new StatCounter(name, count);
                yield return stat;
            }
        }
    }
}
