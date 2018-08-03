using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Text;
using Cassandra;
using Elders.Cronus;
using Elders.Cronus.EventStore;
using Elders.Cronus.IntegrityValidation;
using Elders.Cronus.Migration.Middleware;
using Elders.Cronus.Persistence.Cassandra;
using PushNotifications.Contracts;
using PushNotifications.Contracts.PushNotifications;
using Vapt.EventStore.Migrations;
using Vapt.Migration.Executor.Logging;

namespace Vapt.Migration.Executor
{
    public class Program
    {
        static ILog log = LogProvider.GetLogger(typeof(Program));

        private static List<IMigration<AggregateCommit, AggregateCommit>> migrations;

        private static Dictionary<byte[], int> revisions = new Dictionary<byte[], int>();

        public static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            migrations = new List<IMigration<AggregateCommit, AggregateCommit>>();
            // migrations.Add(new TimestampMigration());

            var oldConnectionString = ConfigurationManager.AppSettings.Get("old_connection_string");
            var newConnectionString = ConfigurationManager.AppSettings.Get("new_connection_string");

            log.Info($"Old connection string: {oldConnectionString}");
            log.Info($"New connection string: {newConnectionString}");

            log.Info($"Migration started.");
            MigrateEventStore(typeof(PushNotificationsContractsAssembly).Assembly, oldConnectionString, newConnectionString);

            log.Info($"Migration finished.");

            Console.ReadLine();
        }

        private static void MigrateEventStore(Assembly contractsAssembly, string oldConnectionString, string newConnectionString)
        {
            int counter = 0;
            try
            {
                var oldPlayer = GetPlayer(contractsAssembly, oldConnectionString, false);
                var newEventStore = GetEventStore(contractsAssembly, newConnectionString, true);
                foreach (var oldCommit in oldPlayer.LoadAggregateCommits(1000))
                {
                    counter++;
                    if (counter % 10000 == 0)
                        log.Debug(counter.ToString());

                    string urnString = Encoding.UTF8.GetString(oldCommit.AggregateRootId);
                    if (urnString.StartsWith("urn:pruvit:pushnotification", StringComparison.Ordinal))
                        continue;

                    newEventStore.Append(oldCommit);
                }

                var aggregateCommits = new List<AggregateCommit>();
                var newPlayer = GetPlayer(contractsAssembly, newConnectionString, false);
                byte[] currentId = { 0 };
                foreach (var commit in newPlayer.LoadAggregateCommits(1000))
                {
                    if (ByteArrayHelper.Compare(currentId, commit.AggregateRootId) == false)
                    {
                        if (aggregateCommits.Count > 0)
                        {
                            RunChecks(new EventStream(aggregateCommits));
                            aggregateCommits.Clear();
                        }
                    }

                    aggregateCommits.Add(commit);
                    currentId = commit.AggregateRootId;
                }
            }
            catch (Exception ex)
            {
                log.FatalException($"Error while migrating event store from {oldConnectionString} to {newConnectionString}", ex);
            }
        }

        private static void RunChecks(EventStream stream)
        {
            var revValidator = new DuplicateRevisionsValidator();
            var missingRevValidator = new MissingRevisionsValidator();
            var orderValidator = new OrderedRevisionsValidator();

            var integrityPolicy = new EventStreamIntegrityPolicy();
            integrityPolicy.RegisterRule(new IntegrityRule<EventStream>(revValidator, new EmptyResolver()));
            integrityPolicy.RegisterRule(new IntegrityRule<EventStream>(missingRevValidator, new EmptyResolver()));
            integrityPolicy.RegisterRule(new IntegrityRule<EventStream>(orderValidator, new EmptyResolver()));

            var result = integrityPolicy.Apply(stream);
            if (result.IsIntegrityViolated)
            {
                Environment.Exit(1);
            }
        }

        private static CassandraEventStorePlayer GetPlayer(Assembly contractsAssembly, string connectionString, bool createKeyspace)
        {
            Assembly[] contractsWithCronus = { contractsAssembly, typeof(AggregateCommit).Assembly };
            var boundedContext = contractsAssembly.GetBoundedContext().BoundedContextName;
            var eventStoreTableNameStrategy = new TablePerBoundedContext(contractsAssembly);
            var serializer = new Elders.Cronus.Serialization.NewtonsoftJson.JsonSerializer(contractsWithCronus);
            var session = CreateSession(connectionString, createKeyspace);
            var player = new CassandraEventStorePlayer(session, eventStoreTableNameStrategy, boundedContext, serializer);
            return player;
        }

        private static CassandraEventStore GetEventStore(Assembly contractsAssembly, string connectionString, bool createKeyspace)
        {
            Assembly[] contractsWithCronus = { contractsAssembly, typeof(AggregateCommit).Assembly };
            var eventStoreTableNameStrategy = new TablePerBoundedContext(contractsAssembly);
            var serializer = new Elders.Cronus.Serialization.NewtonsoftJson.JsonSerializer(contractsWithCronus);
            var session = CreateSession(connectionString, createKeyspace);

            if (createKeyspace)
            {
                var builder = new CassandraConnectionStringBuilder(connectionString);
                connectionString = connectionString.Replace(builder.DefaultKeyspace, "");
                var storageManager = new CassandraEventStoreStorageManager(session, builder.DefaultKeyspace, eventStoreTableNameStrategy, new Elders.Cronus.Persistence.Cassandra.ReplicationStrategies.SimpleReplicationStrategy(1));
                storageManager.CreateStorage();
            }

            var eventStore = new CassandraEventStore("Pushnotifications", session, eventStoreTableNameStrategy, serializer, Cassandra.ConsistencyLevel.Any, Cassandra.ConsistencyLevel.Any);
            return eventStore;
        }
        private static Cassandra.ISession CreateSession(string connectionString, bool createKeyspace)
        {
            var settings = new Cassandra.CassandraConnectionStringBuilder(connectionString);
            var cluster = Cassandra.Cluster.Builder()
                .AddContactPoints(settings.ContactPoints)
                .WithPort(settings.Port)
                .Build();

            var session = cluster.Connect();

            if (createKeyspace)
                session.CreateKeyspaceIfNotExists(settings.DefaultKeyspace);

            session.ChangeKeyspace(settings.DefaultKeyspace);
            return session;
        }
    }
}
