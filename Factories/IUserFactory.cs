using LucaHome.Repositories;

namespace LucaHome.Factories
{
    public interface IUserFactory
    {
        public IUserRepository GetRepository(string targetDb);
    }
}
