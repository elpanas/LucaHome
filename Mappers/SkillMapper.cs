using AutoMapper;
using LucaHome.DTO;
using LucaHome.Models;

namespace LucaHome.Mappers
{
    public class SkillMapper : Profile
    {
        public SkillMapper()
        {
            CreateMap<SkillDTOIn, Skill>();
            CreateMap<Skill, SkillDTOOut>();
        }
    }
}
