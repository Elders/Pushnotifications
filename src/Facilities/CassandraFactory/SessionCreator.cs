namespace Facilities.CassandraFactory
{
    public class SessionFactory
    {
        public static Cassandra.ISession Create(string connectionString)
        {
            var cluster = Cassandra.Cluster.Builder()
                .WithConnectionString(connectionString)
                .Build();
            var session = cluster.ConnectAndCreateDefaultKeyspaceIfNotExists();
            return session;
        }
    }
}