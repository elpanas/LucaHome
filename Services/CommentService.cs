using MongoDB.Driver;
using ProvaRest.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver.Linq;

namespace ProvaRest.Services
{
    public class CommentService
    {
        private readonly IMongoCollection<Comment> _commentsCollection;        

        public CommentService(IOptions<CommentDatabaseSettings> CommentDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                CommentDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                CommentDatabaseSettings.Value.DatabaseName);

            _commentsCollection = mongoDatabase.GetCollection<Comment>(
                CommentDatabaseSettings.Value.CommentCollectionName);
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

        public async Task<bool> CommentExists(string id)
        {
            var query = from c in _commentsCollection.AsQueryable()
                        where c.Id == id
                        select c;
            var result = await query.FirstAsync();

            return (result.Id != null);
        }        
    }
}
