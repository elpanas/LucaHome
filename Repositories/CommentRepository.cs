using LucaHome.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace LucaHome.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly IMongoCollection<Comment> _commentsCollection;

        public CommentRepository(IOptions<DatabaseSettings> DbSettings)
        { 
            var mongoClient = new MongoClient(
                DbSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                DbSettings.Value.DatabaseName);

            _commentsCollection = mongoDatabase.GetCollection<Comment>(
                DbSettings.Value.CommentCollectionName); 
        }

        public async Task<Comment> GetByIdAsync(string id) =>
             await _commentsCollection.Find(c => c.Id == id).FirstAsync();

        public async Task<List<Comment>> GetAllAsync() =>
            await _commentsCollection.Find(_ => true).ToListAsync();

        public async Task CreateAsync(Comment comment) =>
            await _commentsCollection.InsertOneAsync(comment);
    }
}
