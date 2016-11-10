using System;
using System.IO;
using ActivityStreams;
using ActivityStreams.Persistence;
using ActivityStreams.Persistence.Cassandra;

namespace Facilities.Activities
{
    public class ActivityFeedStorage
    {
        public ActivityFeedStorage(Cassandra.ISession session, Func<Elders.Cronus.Serializer.ISerializer> serializer)
        {
            var storageManager = new StorageManager(session);
            storageManager.CreateActivitiesStorage();
            storageManager.CreateStreamsStorage();

            var activityStore = new ActivityStore(session, new ActivityStreamsSerializer(serializer));
            var streamStore = new StreamStore(session);
            var streamRepository = new DefaultStreamRepository(streamStore);
            ActivityRepository = new DefaultActivityRepository(activityStore, streamStore);
            StreamService = new StreamService(streamRepository);
        }

        public IActivityRepository ActivityRepository { get; private set; }

        public StreamService StreamService { get; private set; }

        class ActivityStreamsSerializer : ISerializer
        {
            Func<Elders.Cronus.Serializer.ISerializer> serializer;

            public ActivityStreamsSerializer(Func<Elders.Cronus.Serializer.ISerializer> serializer)
            {
                this.serializer = serializer;
            }

            public object Deserialize(Stream str)
            {
                return this.serializer().Deserialize(str);
            }

            public void Serialize<T>(Stream str, T message)
            {
                this.serializer().Serialize<T>(str, message);
            }
        }
    }
}