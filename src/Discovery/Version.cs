using System;

namespace Discovery
{
    public class DiscoveryVersion
    {
        public DiscoveryVersion(string introducedAtVersion, string depricatedAtVersion)
        {
            if (string.IsNullOrEmpty(introducedAtVersion) == true) throw new ArgumentNullException(nameof(introducedAtVersion));
            if (depricatedAtVersion == null) depricatedAtVersion = string.Empty;

            IntroducedAtVersion = introducedAtVersion;
            DepricatedAtVersion = depricatedAtVersion;
        }

        public DiscoveryVersion(string introducedAtVersion) : this(introducedAtVersion, string.Empty) { }

        public string IntroducedAtVersion { get; private set; }

        public string DepricatedAtVersion { get; private set; }
    }
}
