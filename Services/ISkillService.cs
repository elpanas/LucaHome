using LucaHome.Models;

namespace LucaHome.Services
{
    public interface ISkillService
    {
        Task<Skill> GetSkill(string id);
        Task<List<Skill>> GetSkills();
        Task CreateSkill(Skill skill);
    }
}
