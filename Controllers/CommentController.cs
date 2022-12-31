using Microsoft.AspNetCore.Mvc;
using ProvaRest.Models;
using ProvaRest.Services;

namespace ProvaRest.Controllers
{           
    [Route("api/comment")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly CommentService _commentsService;

        public CommentController(CommentService CommentsService) =>
            _commentsService = CommentsService;
        
        private async Task<bool> CheckComment(string id)
        {
            return await _commentsService.CommentExists(id);
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
        public async Task<ActionResult<List<Comment>>> Get()
        {
            var comments = await _commentsService.GetComments();

            if (comments != null)
                return Ok(comments);
            else
                return NotFound("Commento non presente");
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
