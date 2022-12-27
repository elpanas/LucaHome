using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using ProvaRest.Models;
using ProvaRest.Services;

namespace ProvaRest.Controllers
{
    [EnableCors("My Policy")]
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

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<string> GetAll() {
            return Ok("Benvenuto nel web service della mia pagina personale");
        }        

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Comment>> Post(Comment comment)
        {
            await _commentsService.CreateComment(comment);
           return CreatedAtAction(nameof(Get), new { id = comment.Id }, comment);
        }
            
    }
}
