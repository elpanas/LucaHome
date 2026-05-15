using LucaHome.Factories;
using LucaHome.Models;
using LucaHome.Repositories;
using Microsoft.Extensions.Options;

namespace LucaHome.Services
{
    public class SkillService : ISkillService
    {
        private readonly ISkillRepository _skillRepository;

        public SkillService(ISkillFactory skillFactory, IOptions<DatabaseSettings> dbSettings)
        {
            _skillRepository = skillFactory.GetRepository(dbSettings.Value.DbProvider);
        }

        public async Task<Skill> GetSkill(string id) => await _skillRepository.GetByIdAsync(id);        

        public async Task<List<Skill>> GetSkills() => await _skillRepository.GetAllAsync();        

        public async Task CreateSkill(Skill skill) => await _skillRepository.CreateAsync(skill);
        
    }
}
