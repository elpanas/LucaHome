using LucaHome.Models;
using LucaHome.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace LucaHome.Controllers
{           
    [Route("api/skill")]
    [ApiController]
    public class SkillController : ControllerBase
    {
        private readonly ISkillService _skillsService;

        public SkillController(ISkillService skillsService)
        {
            _skillsService = skillsService;
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
        
        [HttpGet]
        [OutputCache(Duration = 600, Tags = new[] { "tag-skills" })]
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
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize]
        public async Task<ActionResult<Skill>> Post([FromBody]Skill skill, IOutputCacheStore cacheStore)
        {
            if (skill == null)
                return BadRequest();            

            try
            {
                await _skillsService.CreateSkill(skill);

                await cacheStore.EvictByTagAsync("tag-skills", default); // elimina il contenuto cache con il tag "tag-skill"

                return CreatedAtRoute("GetSkill", new { id = skill.Id }, skill);
            } catch 
            {
                return StatusCode(500, "Errore durante il salvataggio o l'aggiornamento della cache.");
            }
            
                    
        }            
    }
}
