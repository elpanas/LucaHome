using Microsoft.AspNetCore.Mvc;
using ProvaRest.Models;
using ProvaRest.Services;

namespace ProvaRest.Controllers
{
    [ApiController]
    [Route("api/comment")]
    public class CommentController : ControllerBase
    {
        private readonly CommentService _commentsService;

        public CommentController(CommentService CommentsService) =>
            _commentsService = CommentsService;

        [HttpGet("id/{id}")]
        public async Task<ActionResult<Comment>> Get(string id)
        {
            var comment = await _commentsService.GetComment(id);
            return Ok(comment);
        }            

        [HttpPost]
        public async Task<IActionResult> Post(Comment comment)
        {
            await _commentsService.CreateComment(comment);
           return CreatedAtAction(nameof(Get), new { id = comment.Id }, comment);
        }
            
    }
}
