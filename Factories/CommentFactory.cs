using LucaHome.Repositories;
using LucaHome.Repositories.Mongo;
using LucaHome.Repositories.SQL;

namespace LucaHome.Factories
{
    public class CommentFactory : ICommentFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public CommentFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ICommentRepository GetRepository(string targetDb)
        {
            return targetDb.ToLower() switch
            {
                "mongodb" => _serviceProvider.GetRequiredService<CommentRepoMongo>(),
                "sql" => _serviceProvider.GetRequiredService<CommentRepoSQL>(), 
                _ => throw new ArgumentException($"Unsupported database type: {targetDb}")
            };
        }
    }
}
