using LucaHome.Models;
using LucaHome.Repositories;

namespace LucaHome.Services
{
    public class SkillService : ISkillService
    {
        private readonly ISkillRepository _skillRepository;

        public SkillService(ISkillRepository skillRepository)
        {
            _skillRepository = skillRepository;
        }

        public async Task<Skill> GetSkill(string id) => await _skillRepository.GetByIdAsync(id);        

        public async Task<List<Skill>> GetSkills() => await _skillRepository.GetAllAsync();        

        public async Task CreateSkill(Skill skill) => await _skillRepository.CreateAsync(skill);
        
    }
}
