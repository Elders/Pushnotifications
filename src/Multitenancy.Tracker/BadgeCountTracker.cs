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
        const string SetCountTemplate = @"UPDATE ""pushnot_badges"" SET cv = ? WHERE subscriberId=?;";

        private readonly ISession _session;
        private PreparedStatement _incrementTemplate;
        private PreparedStatement _setCountTemplate;

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

        private void IncrementTrack(BadgeStatTracker stat)
        {
            if (stat is null) throw new ArgumentNullException(nameof(stat));

            PreparedStatement query = IncrementTemplateStatement;
            _session.Execute(query.Bind(stat.SubscriberId));
        }

        public void SetCount(string subscriberId, int badgeCount)
        {
            if (string.IsNullOrEmpty(subscriberId)) throw new ArgumentNullException(nameof(subscriberId));

            var stat = new StatCounter(subscriberId, badgeCount);
            SetCountTrack(stat);
        }

        private void SetCountTrack(StatCounter stat)
        {
            if (stat is null) throw new ArgumentNullException(nameof(stat));

            PreparedStatement query = SetCountTemplateStatement;
            _session.Execute(query.Bind(stat.Count, stat.Name));

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
