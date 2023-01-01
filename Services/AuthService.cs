using MongoDB.Driver;
using Microsoft.Extensions.Options;
using MongoDB.Driver.Linq;
using LucaHome.Models;
using System.Text;
using System.Security.Cryptography;

namespace LucaHome.Services
{
    public class AuthService
    {
        private readonly IMongoCollection<User> _usersCollection;        

        public AuthService(IOptions<DatabaseSettings> DbSettings)
        {
            var mongoClient = new MongoClient(
                DbSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                DbSettings.Value.DatabaseName);

            _usersCollection = mongoDatabase.GetCollection<User>(
                DbSettings.Value.UserCollectionName);
        }                  

        public async Task<bool> CheckUser(HttpRequest req)
        {
            try
            {
                var accessToken = req.Headers["Authorization"].ToString();

                var bytes = Convert.FromBase64String(accessToken);
                var credentials = Encoding.UTF8.GetString(bytes);

                if (!string.IsNullOrEmpty(credentials))
                {
                    string[] array = credentials.Split(":");
                    string username = array[0];
                    var password = Encoding.UTF8.GetBytes(array[1]);

                    var hash = Convert.ToBase64String(password);

                    var query = from u in _usersCollection.AsQueryable()
                                where u.Username == username && u.Password == hash
                                select u;


                    var user = await query.FirstAsync();                    
                    return true;
                }
                else
                    return false;
            } catch
            {
                return false;
            }
        }       
    }
}
