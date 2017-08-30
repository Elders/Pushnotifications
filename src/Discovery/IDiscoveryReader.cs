using System.Collections.Generic;

namespace Discovery
{
    public interface IDiscoveryReader
    {
        IList<DiscoverableEndpoint> Get();
    }
}
