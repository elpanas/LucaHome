using MongoDB.Bson.Serialization.Attributes;

namespace LucaHome.DTO
{
    public class UserDTOIn
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Ciphertext { get; set; } // Aggiunto per il ciphertext PQC
    }
}
