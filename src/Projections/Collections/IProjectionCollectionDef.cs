using Elders.Cronus.DomainModeling;
using Elders.Cronus.DomainModeling.Projections;

namespace Projections.Collections
{
    public interface IProjectionCollectionDef<out T> : IProjection
        where T : IProjectionCollectionState
    {
        T State { get; }
    }
}