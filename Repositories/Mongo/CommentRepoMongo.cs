using LucaHome.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace LucaHome.Repositories.Mongo
{
    public class CommentRepoMongo : ICommentRepository
    {
        private readonly IMongoCollection<Comment> _commentsCollection;

        public CommentRepoMongo(IOptions<MongoSettings> DbSettings)
        { 
            var mongoClient = new MongoClient(
                DbSettings.Value.ConnectionStringMongo);

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
