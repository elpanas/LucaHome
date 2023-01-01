using MongoDB.Driver;
using LucaHome.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver.Linq;

namespace LucaHome.Services
{
    public class CommentService
    {
        private readonly IMongoCollection<Comment> _commentsCollection;        

        public CommentService(IOptions<DatabaseSettings> DbSettings)
        {
            var mongoClient = new MongoClient(
                DbSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                DbSettings.Value.DatabaseName);

            _commentsCollection = mongoDatabase.GetCollection<Comment>(
                DbSettings.Value.CommentCollectionName);
        }                  

        public async Task<Comment> GetComment(string id)
        {
            var query = from c in _commentsCollection.AsQueryable()
                        where c.Id == id
                        select c;

            return await query.FirstAsync();
        }

        public async Task<List<Comment>> GetComments()
        {            
            var query = from c in _commentsCollection.AsQueryable()                        
                        select c;

            return await query.ToListAsync();
        }

        public async Task CreateComment(Comment comment) => await _commentsCollection.InsertOneAsync(comment);           
    }
}
