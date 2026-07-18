using LucaHome.DBs;
using LucaHome.Models;
using Microsoft.EntityFrameworkCore;

namespace LucaHome.Repositories.SQL
{
    public class SkillRepoSQL : ISkillRepository
    {
        private readonly SQLDBContext _context;
        public SkillRepoSQL(SQLDBContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(Skill skill) =>         
            await _context.Skills.AddAsync(skill);
        

        public async Task<List<Skill>> GetAllAsync()
        {
            return await _context.Skills
                .OrderBy(s => s.Category)
                .ThenBy(s => s.Sequential)
                .AsNoTracking() // Aggiunto AsNoTracking per migliorare le prestazioni quando non è necessario il tracking delle entità
                .ToListAsync();
        }

        public async Task<Skill?> GetByIdAsync(string id) => 
            await _context.Skills.FindAsync(id);
    }
}
