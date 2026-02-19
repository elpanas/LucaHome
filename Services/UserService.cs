using LucaHome.DTO;
using LucaHome.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace LucaHome.Services
{
    public class UserService : IUserService
    {
        private readonly IMongoCollection<User> _usersCollection;

        public UserService(IOptions<DatabaseSettings> DbSettings)
        {
            var mongoClient = new MongoClient(
                DbSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                DbSettings.Value.DatabaseName);

            _usersCollection = mongoDatabase.GetCollection<User>(
                DbSettings.Value.UserCollectionName);
        }

        public async Task<bool> Login(UserDTOIn userDTO)
        {
            // Here you would typically check the email and password against a database
            // For demonstration purposes, we'll just return a new UserDTOIn if the email and password are not empty
            if (!string.IsNullOrEmpty(userDTO.Username) && !string.IsNullOrEmpty(userDTO.Password))
            {
                // Cerco l'utente per email
                var query = from u in _usersCollection.AsQueryable()
                            where u.Username == userDTO.Username
                            select u;

                User user = await query.FirstAsync();

                if (user == null) return false;

                return BCrypt.Net.BCrypt.Verify(userDTO.Password, user.Password);
            }
            return true;
        }
    }
}

