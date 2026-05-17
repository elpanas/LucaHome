using LucaHome.Repositories;

namespace LucaHome.Factories
{
    public interface ICommentFactory
    {
        public ICommentRepository GetRepository(string targetDb);
    }
}
