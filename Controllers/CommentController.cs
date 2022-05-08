using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using ProvaRest.Models;
using ProvaRest.Services;
using System.Net;

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

        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            resp.Headers.Add("Access-Control-Allow-Origin", "*");
            resp.Headers.Add("Access-Control-Allow-Methods", "POST,GET,DELETE");

            return resp;
        }

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
