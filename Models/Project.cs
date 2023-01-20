using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace LucaHome.Models
{
    public class Project
    {
        [BsonId, BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("name")]
        public string? Name { get; set; }

        [BsonElement("description")]
        public string? Description { get; set; }

        [BsonElement("stack")]
        public string? Stack { get; set; }

        [BsonElement("report")]
        public Boolean Report { get; set; }
    }
}
