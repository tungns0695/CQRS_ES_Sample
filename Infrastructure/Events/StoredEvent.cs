using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Infrastructure.Events
{
    public sealed class StoredEvent : IEvent
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
        public Guid AggregateId { get; set; }
        public int Version { get; set; }
        public long Position { get; set; } // Global position for ordering
        public required string TypeName { get; set; }
        public required string FullName { get; set; }
        public required string Data { get; set; }
        public DateTimeOffset OccuredOn { get; set; }

        [BsonIgnore]
        public required IEvent Event { get; set; }
    }
}