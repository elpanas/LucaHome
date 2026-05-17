using LucaHome.Repositories;
using LucaHome.Repositories.Mongo;
using LucaHome.Repositories.SQL;

namespace LucaHome.Factories
{
    public class UserFactory : IUserFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public UserFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IUserRepository GetRepository(string targetDb)
        {
            return targetDb.ToLower() switch
            {
                "mongodb" => _serviceProvider.GetRequiredService<UserRepoMongo>(),
                "sql" => _serviceProvider.GetRequiredService<UserRepoSQL>(),
                _ => throw new ArgumentException($"Unsupported database type: {targetDb}")
            };
        }
    }
}
