using MongoDB.Driver;
using LucaHome.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver.Linq;

namespace LucaHome.Services
{
    public class ProjectService
    {
        private readonly IMongoCollection<Project> _projectsCollection;        

        public ProjectService(IOptions<DatabaseSettings> DbSettings)
        {
            var mongoClient = new MongoClient(
                DbSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                DbSettings.Value.DatabaseName);

            _projectsCollection = mongoDatabase.GetCollection<Project>(
                DbSettings.Value.ProjectCollectionName);
        }                  

        public async Task<Project> GetProject(string id)
        {
            var query = from p in _projectsCollection.AsQueryable()
                        where p.Id == id
                        select p;

            return await query.FirstAsync();
        }

        public async Task<List<Project>> GetProjects()
        {            
            var query = from p in _projectsCollection.AsQueryable()                        
                        select p;

            return await query.ToListAsync();
        }

        public async Task CreateProject(Project project) => await _projectsCollection.InsertOneAsync(project);           
    }
}
