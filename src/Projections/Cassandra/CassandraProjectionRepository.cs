using Elders.Cronus.DomainModeling;
using Elders.Cronus.DomainModeling.Projections;
using Elders.Cronus.Projections.Cassandra;
using Projections.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Projections.Cassandra
{
    public class CassandraProjectionRepository : IProjectionRepository
    {
        readonly ProjectionBuilder builder;

        readonly Func<IRepository> repository;

        public CassandraProjectionRepository(ProjectionBuilder builder, Func<IRepository> repository)
        {
            this.builder = builder;
            this.repository = repository;
        }

        public TProjection Load<TProjection>(object id, IProjectionState defaultState = null)
            where TProjection : IProjectionDef<IProjectionState>
        {
            Type stateType = typeof(TProjection).BaseType.GetGenericArguments().First();

            var state = (IProjectionState)(repository() as Repository).Get(id, stateType);

            return builder.Rebuild<TProjection>(new List<IEvent>(), state ?? defaultState);
        }

        public TProjection TryLoad<TProjection>(object id, IProjectionState defaultState = null)
           where TProjection : IProjectionDef<IProjectionState>
        {
            Type stateType = typeof(TProjection).BaseType.GetGenericArguments().First();

            var state = (IProjectionState)(repository() as Repository).Get(id, stateType);

            return builder.Rebuild<TProjection>(new List<IEvent>(), state ?? defaultState);
        }

        public IEnumerable<TProjection> LoadCollectionItems<TProjection>(object collectionId, IProjectionState defaultState = null)
            where TProjection : IProjectionCollectionDef<IProjectionCollectionState>
        {
            Type stateType = typeof(TProjection).BaseType.GetGenericArguments().First();
            var repo = (repository() as Repository);
            var collectionItems = repo.GetAsCollectionItems(collectionId, stateType);
            var states = collectionItems.ToList();


            if (states.Count == 0)
                yield return builder.Rebuild<TProjection>(new List<IEvent>(), defaultState);

            foreach (IProjectionCollectionState state in states)
            {
                var rebuilded = builder.Rebuild<TProjection>(new List<IEvent>(), state ?? defaultState);
                if (ReferenceEquals(rebuilded, default(TProjection)) == true) continue;
                yield return rebuilded;
            }
        }

        public TProjection LoadCollectionItem<TProjection>(object collectionId, object itemId, IProjectionState defaultVal = null) where TProjection : IProjectionCollectionDef<IProjectionCollectionState>
        {
            Type stateType = typeof(TProjection).BaseType.GetGenericArguments().First();
            var state = (IProjectionState)(repository() as Repository).GetAsCollectionItem(collectionId, itemId, stateType);

            var rebuilded = builder.Rebuild<TProjection>(new List<IEvent>(), state ?? defaultVal);
            return rebuilded;

        }

        public void Save<TProjection>(TProjection projection)
            where TProjection : IProjectionDef<IProjectionState>
        {
            (repository() as Repository).Save(projection.State.Id, projection.State);
        }

        public void SaveAsCollection<TProjection>(TProjection projection) where TProjection : IProjectionCollectionDef<IProjectionCollectionState>
        {
            (repository() as Repository).Save(projection.State.CollectionId, projection.State.Id, projection.State);
        }

        public void Delete<TProjection>(object id) where TProjection : IProjectionDef<IProjectionState>
        {
            Type stateType = typeof(TProjection).BaseType.GetGenericArguments().First();

            (repository() as Repository).Delete(id, stateType);
        }

        public void DeleteCollectionItem<TProjection>(object collectionId, object itemId) where TProjection : IProjectionCollectionDef<IProjectionCollectionState>
        {
            Type stateType = typeof(TProjection).BaseType.GetGenericArguments().First();

            (repository() as Repository).DeleteCollectionItem(collectionId, itemId, stateType);
        }
    }
}