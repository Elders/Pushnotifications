using Elders.Cronus.DomainModeling.Projections;
using System.Runtime.Serialization;

namespace Projections.Collections
{
    [DataContract(Name = "e32ca699-8803-4e4c-9798-aac7ecfa4001")]
    public class ProjectionCollectionState<Value, Key> : ICollectionDataTransferObjectItem<Value, Key>
    {
        object IProjectionCollectionState.CollectionId { get { return CollectionId; } }

        object IProjectionState.Id { get { return Id; } }

        [DataMember(Order = 99)]
        public Key CollectionId { get; set; }

        [DataMember(Order = 100)]
        public Value Id { get; set; }
    }
}