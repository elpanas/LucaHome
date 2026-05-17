using LucaHome.Repositories;
using LucaHome.Repositories.Mongo;
using LucaHome.Repositories.SQL;

namespace LucaHome.Factories
{
    public class SkillFactory : ISkillFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public SkillFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ISkillRepository GetRepository(string targetDb)
        {
            return targetDb.ToLower() switch
            {
                "mongodb" => _serviceProvider.GetRequiredService<SkillRepoMongo>(),
                "sql" => _serviceProvider.GetRequiredService<SkillRepoSQL>(),
                _ => throw new Exception("Database non supportato")
            };
        }
    }
}

