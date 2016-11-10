using Elders.Cronus.DomainModeling.Projections;
using System.Runtime.Serialization;

namespace Projections
{
    [DataContract(Name = "f6b1719a-d85f-458d-8983-805d1397e13f")]
    public class ProjectionState<ID> : IDataTransferObject<ID>
    {
        object IProjectionState.Id { get { return Id; } }

        [DataMember(Order = 100)]
        public ID Id { get; set; }
    }
}