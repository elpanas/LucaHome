using Microsoft.AspNetCore.Mvc;
using LucaHome.Models;
using LucaHome.Services;

namespace LucaHome.Controllers
{           
    [Route("api/comment")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly CommentService _commentsService;
        private readonly AuthService _authService;

        public CommentController(CommentService commentsService, AuthService authService)
        {
            _commentsService = commentsService;
            _authService = authService;
        }   

        [HttpGet("id/{id}", Name = "GetComment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Comment>> Get(string id)
        {
            var comment = await _commentsService.GetComment(id);

            if (comment != null) 
                return Ok(comment);
            else
                return NotFound("Commento non presente");
        }
        
        [HttpGet("comments")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<Comment>>> Get()
        {     
            var check = await _authService.CheckUser(Request);

            if (check)
            {
                var comments = await _commentsService.GetComments();

                if (comments != null)
                    return Ok(comments);
                else
                    return NotFound("Commento non presenti");
            } else            
                return Unauthorized("Unauthorized");                        
        }
        
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]     
        [ProducesResponseType(StatusCodes.Status400BadRequest)]  
        public async Task<ActionResult<Comment>> Post([FromBody]Comment comment)
        {
            if (comment == null)
                return BadRequest(comment);
            else
            {
                await _commentsService.CreateComment(comment);
                return CreatedAtRoute("GetComment", new { id = comment.Id }, comment);
            }            
        }            
    }
}
