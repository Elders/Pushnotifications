using System;

namespace Discovery
{
    public class DiscoverableEndpoint
    {
        public DiscoverableEndpoint(string name, Uri uri, string boundedContext, DiscoveryVersion version)
        {
            if (string.IsNullOrEmpty(name) == true) throw new ArgumentNullException(nameof(name));
            if (ReferenceEquals(null, uri) == true) throw new ArgumentNullException(nameof(uri));
            if (string.IsNullOrEmpty(boundedContext) == true) throw new ArgumentNullException(nameof(boundedContext));
            if (ReferenceEquals(null, version) == true) throw new ArgumentNullException(nameof(version));

            Name = name;
            Url = uri;
            BoundedContext = boundedContext;
            Version = version;
        }

        public string Name { get; private set; }

        public Uri Url { get; private set; }

        public string BoundedContext { get; private set; }

        public DiscoveryVersion Version { get; private set; }
    }
}
