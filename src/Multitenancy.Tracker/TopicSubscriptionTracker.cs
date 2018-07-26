using Cassandra;
using Elders.Cronus.AtomicAction;
using PushNotifications.Contracts;
using System;
using System.Collections.Generic;

namespace Multitenancy.Tracker
{
    public partial class TopicSubscriptionTracker : ITopicSubscriptionTracker
    {
        const string CreateTableTemplateLockKey = "PushNotifications_CreateTableTemplateLockKey";
        const string CreateTableTemplate = @"CREATE TABLE IF NOT EXISTS ""pushnot_subscriptions"" (cv counter, name varchar, PRIMARY KEY (name));";
        const string IncrementTemplate = @"UPDATE ""pushnot_subscriptions"" SET cv = cv + 1 WHERE name=?;";
        const string DecrementTemplate = @"UPDATE ""pushnot_subscriptions"" SET cv = cv - 1 WHERE name=?;";
        const string GetTemplate = @"SELECT * FROM pushnot_subscriptions WHERE name=?;";

        private readonly ISession _session;
        private readonly PreparedStatement incrementTemplate;
        private readonly PreparedStatement decrementTemplate;
        private readonly PreparedStatement getTemplate;

        public TopicSubscriptionTracker(ISession session, ILock @lock)
        {
            if (ReferenceEquals(null, session)) throw new ArgumentNullException(nameof(session));
            if (ReferenceEquals(null, @lock)) throw new ArgumentNullException(nameof(@lock));

            _session = session;
            var ttl = TimeSpan.FromSeconds(2);

            CreateTableWithLock(_session, @lock, ttl);

            incrementTemplate = session.Prepare(IncrementTemplate);
            decrementTemplate = session.Prepare(DecrementTemplate);
            getTemplate = session.Prepare(GetTemplate);
        }

        private void CreateTableWithLock(ISession session, ILock @lock, TimeSpan ttl)
        {
            if (@lock.Lock(CreateTableTemplateLockKey, ttl))
            {
                try
                {
                    session.Execute(CreateTableTemplate);
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    @lock.Unlock(CreateTableTemplateLockKey);
                }
            }
        }

        public void Increment(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            var stat = new TopicSubscriptionStat(name);
            IncrementTrack(stat);
        }

        public void Decrement(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            var stat = new TopicSubscriptionStat(name);
            DecrementTrack(stat);
        }

        private void IncrementTrack(TopicSubscriptionStat stat)
        {
            if (ReferenceEquals(null, stat)) throw new ArgumentNullException(nameof(stat));

            PreparedStatement query = incrementTemplate;
            _session.Execute(query.Bind(stat.Name));
        }

        private void DecrementTrack(TopicSubscriptionStat stat)
        {
            if (ReferenceEquals(null, stat)) throw new ArgumentNullException(nameof(stat));

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
