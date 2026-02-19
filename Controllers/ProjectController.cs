using Microsoft.AspNetCore.Mvc;
using LucaHome.Models;
using LucaHome.Services;
using Microsoft.AspNetCore.Authorization;

namespace LucaHome.Controllers
{           
    [Route("api/project")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly ProjectService _projectsService;

        public ProjectController(ProjectService projectsService)
        {
            _projectsService = projectsService;            
        }   

        [HttpGet("id/{id}", Name = "GetProject")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Project>> Get(string id)
        {
            var project = await _projectsService.GetProject(id);

            if (project != null) 
                return Ok(project);
            else
                return NotFound("Progetto non presente");
        }
        
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize]
        public async Task<ActionResult<List<Project>>> Get()
        {
            var projects = await _projectsService.GetProjects();

            if (projects != null)
                return Ok(projects);
            else
                return NotFound("Progetti non presenti");                        
        }
        
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]     
        [ProducesResponseType(StatusCodes.Status400BadRequest)]  
        public async Task<ActionResult<Project>> Post([FromBody]Project project)
        {
            if (project == null)
                return BadRequest(project);
            else
            {
                await _projectsService.CreateProject(project);
                return CreatedAtRoute("GetProject", new { id = project.Id }, project);
            }            
        }            
    }
}
