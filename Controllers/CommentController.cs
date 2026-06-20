using Microsoft.AspNetCore.Mvc;
using LucaHome.Models;
using LucaHome.Services;
using Microsoft.AspNetCore.Authorization;
using LucaHome.DTO;
using AutoMapper;
using Microsoft.AspNetCore.RateLimiting;

namespace LucaHome.Controllers
{           
    [Route("api/comment")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentsService;
        private readonly IMapper _mapper;

        public CommentController(ICommentService commentsService, IMapper mapper)
        {
            _commentsService = commentsService;
            _mapper = mapper;
        }   

        [HttpGet("id/{id}", Name = "GetComment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CommentDTOOut>> Get(string id)
        {
            var comment = await _commentsService.GetComment(id);

            if (comment != null)
            {
                var commentDTOOut = _mapper.Map<CommentDTOOut>(comment);
                return Ok(commentDTOOut);
            }
            else
                return NotFound("Commento non presente");
        }
        
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize]
        public async Task<ActionResult<List<CommentDTOOut>>> Get()
        {     
            var comments = await _commentsService.GetComments();

            if (comments != null)
                return Ok(comments);
            else
                return NotFound("Commenti non presenti");   
        }
        
        [HttpPost]
        [EnableRateLimiting("strict")]
        [ProducesResponseType(StatusCodes.Status201Created)]     
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CommentDTOIn>> Post([FromBody]CommentDTOIn commentIn)
        {
            if (commentIn == null)
                return BadRequest(commentIn);

            if (!string.IsNullOrEmpty(commentIn.Middlename))            
                return Ok(); // Risponde 200 (fai credere al bot di aver vinto) ma non salva nulla nel DB            
          
            var comment = _mapper.Map<Comment>(commentIn);
            await _commentsService.CreateComment(comment);
            return CreatedAtRoute("GetComment", new { id = comment.Id }, comment);
                       
        }            
    }
}
