using LucaHome.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace LucaHome.Repositories.Mongo
{
    public class SkillRepoMongo : ISkillRepository
    {
        private readonly IMongoCollection<Skill> _skillsCollection;

        public SkillRepoMongo(IOptions<DBSettings> DbSettings)
        {
            var mongoClient = new MongoClient(
                DbSettings.Value.ConnectionStringMongo);

            var mongoDatabase = mongoClient.GetDatabase(
                DbSettings.Value.DatabaseName);

            _skillsCollection = mongoDatabase.GetCollection<Skill>(
                DbSettings.Value.SkillCollectionName);
        }

        public async Task<Skill> GetByIdAsync(string id) =>
             await _skillsCollection.Find(s => s.Id == id).FirstAsync();


        public async Task<List<Skill>> GetAllAsync()
        {
            return await _skillsCollection.Find(_ => true)
                .SortBy(s => s.Category)
                .ThenBy(s => s.Sequential)
                .ToListAsync();
        }

        public async Task CreateAsync(Skill skill) =>
            await _skillsCollection.InsertOneAsync(skill);
    }
}