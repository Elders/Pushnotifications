using Elders.Cronus.DomainModeling.Projections;
using System;

namespace Projections
{
    public abstract class ProjectionDef<T> : IProjectionDef<T>
       where T : IProjectionState
    {
        public ProjectionDef()
        {
            InitializeEmptyState();
        }

        public T State { get; set; }

        void InitializeEmptyState()
        {
            State = (T)Activator.CreateInstance(typeof(T), true);
        }
    }
}