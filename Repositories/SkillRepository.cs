using LucaHome.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Runtime;

namespace LucaHome.Repositories
{
    public class SkillRepository : ISkillRepository
    {
        private readonly IMongoCollection<Skill> _skillsCollection;

        public SkillRepository(IOptions<DatabaseSettings> DbSettings)
        {
            var mongoClient = new MongoClient(
                DbSettings.Value.ConnectionString);

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
