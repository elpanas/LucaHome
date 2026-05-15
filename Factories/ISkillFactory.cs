using LucaHome.Repositories;

namespace LucaHome.Factories
{
    public interface ISkillFactory
    {
        public ISkillRepository GetRepository(string targetDb);
    }
}
