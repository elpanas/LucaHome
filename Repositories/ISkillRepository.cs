using LucaHome.Models;

namespace LucaHome.Repositories
{
    public interface ISkillRepository
    {
        Task<Skill?> GetByIdAsync(string id);
        Task<List<Skill>> GetAllAsync();
        Task CreateAsync(Skill skill);
    }
}
