using LucaHome.DTO;
using LucaHome.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace LucaHome.Repositories.Mongo
{
    public class UserRepoMongo : IUserRepository
    {
        private readonly IMongoCollection<User> _usersCollection;

        public UserRepoMongo(IOptions<DBSettings> DbSettings)
        {
            var mongoClient = new MongoClient(DbSettings.Value.ConnectionStringMongo);
            var mongoDatabase = mongoClient.GetDatabase(DbSettings.Value.DatabaseName);
            _usersCollection = mongoDatabase.GetCollection<User>(DbSettings.Value.UserCollectionName);
        }

        public async Task<User?> GetUserByUsername(string username)
        {
            // Cerco l'utente per username
            var query = from u in _usersCollection.AsQueryable()
                        where u.Username == username
                        select u;

            return await query.FirstOrDefaultAsync();
        }
    }
}
