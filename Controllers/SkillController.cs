using Microsoft.AspNetCore.Mvc;
using LucaHome.Models;
using LucaHome.Services;

namespace LucaHome.Controllers
{           
    [Route("api/skill")]
    [ApiController]
    public class SkillController : ControllerBase
    {
        private readonly SkillService _skillsService;
        private readonly AuthService _authService;

        public SkillController(SkillService skillsService, AuthService authService)
        {
            _skillsService = skillsService;
            _authService = authService;
        }   

        [HttpGet("id/{id}", Name = "GetSkill")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Skill>> Get(string id)
        {
            var skill = await _skillsService.GetSkill(id);

            if (skill != null) 
                return Ok(skill);
            else
                return NotFound("Skill non presente");
        }
        
        [HttpGet("skills")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]   
        public async Task<ActionResult<List<Skill>>> Get()
        {    
            var skills = await _skillsService.GetSkills();

            if (skills != null)
                return Ok(skills);
            else
                return NotFound("Skills non presenti");                                                       
        }
        
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]     
        [ProducesResponseType(StatusCodes.Status400BadRequest)]  
        public async Task<ActionResult<Skill>> Post([FromBody]Skill skill)
        {
            if (skill == null)
                return BadRequest(skill);
            else
            {
                await _skillsService.CreateSkill(skill);
                return CreatedAtRoute("GetSkill", new { id = skill.Id }, skill);
            }            
        }            
    }
}
