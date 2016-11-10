using Elders.Cronus.DomainModeling;
using Elders.Cronus.DomainModeling.Projections;

namespace Projections
{
    public interface IProjectionDef<out T>
       where T : IProjectionState
    {
        T State { get; }
    }
}
