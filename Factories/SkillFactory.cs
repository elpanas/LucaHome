using LucaHome.Repositories;

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
                // "sql" => _serviceProvider.GetRequiredService<SqlSkillRepository>(),
                _ => throw new Exception("Database non supportato")
            };
        }
    }
}

