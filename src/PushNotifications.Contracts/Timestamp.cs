using Elders.Cronus.DomainModeling;
using System;
using System.Runtime.Serialization;

namespace PushNotifications.Contracts
{
    [DataContract(Name = "043b8da3-dfdc-4f43-8087-94b0f61d4abc")]
    public class Timestamp : ValueObject<Timestamp>
    {
        private Timestamp() { }

        public Timestamp(DateTime dateTime)
        {
            if (ReferenceEquals(null, dateTime) == true) throw new ArgumentNullException(nameof(dateTime));
            if (dateTime.Kind != DateTimeKind.Utc) throw new ArgumentException("All timestamps should be utc!");

            FileTimeUtc = dateTime.ToFileTimeUtc();
        }

        [DataMember(Order = 1)]
        public long FileTimeUtc { get; private set; }

        public DateTime DateTime { get { return DateTime.FromFileTimeUtc(FileTimeUtc); } }

        public static Timestamp UtcNow()
        {
            return new Timestamp(DateTime.UtcNow);
        }

        public static Timestamp JudgementDay()
        {
            return new Timestamp(DateTime.UtcNow.AddYears(100));
        }

        public static Timestamp RandomMilliseconds(Timestamp source)
        {
            if (ReferenceEquals(null, source) == true) throw new ArgumentNullException(nameof(source));

            var random = new Random();
            var dateTime = source.DateTime.AddMilliseconds(random.Next(1, 100));
            return new Timestamp(dateTime);
        }
    }
}
