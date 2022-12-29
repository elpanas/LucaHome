using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using ProvaRest.Models;
using ProvaRest.Services;

namespace ProvaRest.Controllers
{
    [EnableCors("MyCorPolicy")]    
    [ApiController]    
    [Route("api/comment")]
    public class CommentController : ControllerBase
    {
        private readonly CommentService _commentsService;

        public CommentController(CommentService CommentsService) =>
            _commentsService = CommentsService;
        
        private async Task<bool> CheckComment(string id)
        {
            return await _commentsService.CommentExists(id);
        }       

        [HttpOptions]
        public IActionResult PreflightRoute()
        {
            return Ok();
        } 
        
        [HttpGet("id/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Comment))]
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Comment>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Comment>> Get()
        {
            var comment = await _commentsService.GetComments();

            if (comment != null)
                return Ok(comment);
            else
                return NotFound("Commento non presente");
        }
        
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Comment>> Post(Comment comment)
        {
            await _commentsService.CreateComment(comment);
           return CreatedAtAction(nameof(Get), new { id = comment.Id }, comment);
        }
            
    }
}
