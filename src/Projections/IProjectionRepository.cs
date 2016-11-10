using Elders.Cronus.DomainModeling.Projections;
using Projections.Collections;
using System.Collections.Generic;

namespace Projections
{
    public interface IProjectionRepository
    {
        TProjection Load<TProjection>(object id, IProjectionState defaultVal = null) where TProjection : IProjectionDef<IProjectionState>;

        IEnumerable<TProjection> LoadCollectionItems<TProjection>(object collectionId, IProjectionState defaultVal = null) where TProjection : IProjectionCollectionDef<IProjectionCollectionState>;

        TProjection LoadCollectionItem<TProjection>(object collectionId, object itemId, IProjectionState defaultVal = null) where TProjection : IProjectionCollectionDef<IProjectionCollectionState>;

        void Save<TProjection>(TProjection projection) where TProjection : IProjectionDef<IProjectionState>;

        void SaveAsCollection<TProjection>(TProjection projection) where TProjection : IProjectionCollectionDef<IProjectionCollectionState>;

        void Delete<TProjection>(object id) where TProjection : IProjectionDef<IProjectionState>;

        void DeleteCollectionItem<TProjection>(object collectionId, object itemId) where TProjection : IProjectionCollectionDef<IProjectionCollectionState>;

    }
}