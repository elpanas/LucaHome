using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace LucaHome.Models
{
    public class Skill
    {
        [BsonId, BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("name")]
        public string? Name { get; set; } 

        [BsonElement("category")]
        public int? Category { get; set; }

        [BsonElement("sequential")]
        public int? Sequential { get; set; }
    }
}
