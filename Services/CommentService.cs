using LucaHome.Models;
using LucaHome.Repositories;

namespace LucaHome.Services
{
    public class CommentService : ICommentService
    {
       public readonly ICommentRepository _commentRepository;

        public CommentService(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }
        public async Task<Comment> GetComment(string id) => await _commentRepository.GetByIdAsync(id);
        public async Task<List<Comment>> GetComments() => await _commentRepository.GetAllAsync();
        public async Task CreateComment(Comment comment) => await _commentRepository.CreateAsync(comment);
    }
}
