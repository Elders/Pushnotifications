using Cassandra;
using Elders.Cronus.AtomicAction;
using PushNotifications.Contracts;
using System;

namespace Multitenancy.Tracker
{
    public class BadgeCountTracker : IBadgeCountTracker
    {
        const string CreateTableTemplateLockKey = "PushNotifications_CreateBadgeCountTableTemplateLockKey";
        const string CreateTableTemplate = @"CREATE TABLE IF NOT EXISTS ""pushnot_badges"" (cv counter, subscriberId varchar, PRIMARY KEY (subscriberId));";
        const string IncrementTemplate = @"UPDATE ""pushnot_badges"" SET cv = cv + 1 WHERE subscriberId=?;";
        const string SetCountTemplate = @"UPDATE ""pushnot_badges"" SET cv = cv - ? WHERE subscriberId=?;";
        const string GetTemplate = @"SELECT * FROM pushnot_badges WHERE subscriberId=?;";

        private readonly ISession _session;
        private PreparedStatement _incrementTemplate;
        private PreparedStatement _setCountTemplate;
        private PreparedStatement _getTemplate;

        private PreparedStatement IncrementTemplateStatement
        {
            get
            {
                if (_incrementTemplate is null)
                    _incrementTemplate = _session.Prepare(IncrementTemplate);
                return _incrementTemplate;
            }
        }
        private PreparedStatement SetCountTemplateStatement
        {
            get
            {
                if (_setCountTemplate is null)
                    _setCountTemplate = _session.Prepare(SetCountTemplate);
                return _setCountTemplate;
            }
        }
        private PreparedStatement GetTemplateStatement
        {
            get
            {
                if (_getTemplate is null)
                    _getTemplate = _session.Prepare(GetTemplate);
                return _getTemplate;
            }
        }

        public BadgeCountTracker(ISession session, ILock @lock)
        {
            if (session is null) throw new ArgumentNullException(nameof(session));
            if (@lock is null) throw new ArgumentNullException(nameof(@lock));

            _session = session;
            var ttl = TimeSpan.FromSeconds(2);

            CreateTableWithLock(_session, @lock, ttl);
        }

        public void Increment(string subscriberId)
        {
            if (string.IsNullOrEmpty(subscriberId)) throw new ArgumentNullException(nameof(subscriberId));

            var stat = new BadgeStatTracker(subscriberId);
            IncrementTrack(stat);
        }

        public void SetCount(string subscriberId, int badgeCount)
        {
            if (string.IsNullOrEmpty(subscriberId)) throw new ArgumentNullException(nameof(subscriberId));

            var stat = new StatCounter(subscriberId, badgeCount);
            SetCountTrack(stat);
        }

        private void IncrementTrack(BadgeStatTracker stat)
        {
            if (stat is null) throw new ArgumentNullException(nameof(stat));

            PreparedStatement query = IncrementTemplateStatement;
            _session.Execute(query.Bind(stat.SubscriberId));
        }

        private void SetCountTrack(StatCounter targetStat)
        {
            if (targetStat is null) throw new ArgumentNullException(nameof(targetStat));

            string subscriberId = targetStat.Name;
            StatCounter databaseStat = Show(subscriberId);

            PreparedStatement query = SetCountTemplateStatement;
            _session.Execute(query.Bind(CalculateCounter(databaseStat.Count, targetStat.Count), subscriberId));
        }

        public StatCounter Show(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            var statCounter = GetStatsTable(name);

            return statCounter;
        }

        private StatCounter GetStatsTable(string name)
        {
            PreparedStatement query = GetTemplateStatement;

            BoundStatement boundedStatement = query.Bind(name);
            RowSet result = _session.Execute(boundedStatement);

            foreach (Row row in result.GetRows())
            {
                long count = row.GetValue<long>("cv");
                var stat = new StatCounter(name, count);
                return stat;
            }

            return StatCounter.Empty(name);
        }

        private long CalculateCounter(long databaseCounter, long setTarget)
        {
            return (databaseCounter - setTarget);
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
    }
}
