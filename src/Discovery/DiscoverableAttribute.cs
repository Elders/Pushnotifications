using System;

namespace Discovery
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class DiscoverableAttribute : Attribute
    {
        public DiscoverableAttribute(string endpointName, string version, string depricateVersion = null)
        {
            EndpointName = endpointName;
            Version = version;
            DepricateVersion = depricateVersion;
        }

        public string EndpointName { get; private set; }

        public string Version { get; private set; }

        public string DepricateVersion { get; private set; }
    }
}
