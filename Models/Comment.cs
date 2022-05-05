using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProvaRest.Models
{
    public class Comment
    {  
        //[BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; } = null!;
        [BsonElement("email")]
        public string Email { get; set; } = null!;
        [BsonElement("content")]
        public string Content { get; set; } = null!;
     
    }
}
