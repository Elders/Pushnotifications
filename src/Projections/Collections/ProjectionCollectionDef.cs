using Elders.Cronus.DomainModeling.Projections;
using System;

namespace Projections.Collections
{
    public abstract class ProjectionCollectionDef<T> : IProjectionCollectionDef<T>
       where T : IProjectionCollectionState
    {
        public ProjectionCollectionDef()
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