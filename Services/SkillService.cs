using MongoDB.Driver;
using LucaHome.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver.Linq;

namespace LucaHome.Services
{
    public class SkillService : ISkillService
    {
        private readonly IMongoCollection<Skill> _skillsCollection;        

        public SkillService(IOptions<DatabaseSettings> DbSettings)
        {
            var mongoClient = new MongoClient(
                DbSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                DbSettings.Value.DatabaseName);

            _skillsCollection = mongoDatabase.GetCollection<Skill>(
                DbSettings.Value.SkillCollectionName);
        }                  

        public async Task<Skill> GetSkill(string id)
        {
            var query = from s in _skillsCollection.AsQueryable()
                        where s.Id == id
                        select s;

            return await query.FirstAsync();
        }

        public async Task<List<Skill>> GetSkills()
        {
            var query = from s in _skillsCollection.AsQueryable() 
                        orderby s.Category
                        orderby s.Sequential
                        select s;                        

            return await query.ToListAsync();
        }

        public async Task CreateSkill(Skill skill) => await _skillsCollection.InsertOneAsync(skill);           
    }
}
