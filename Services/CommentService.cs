using LucaHome.Factories;
using LucaHome.Models;
using LucaHome.Repositories;
using Microsoft.Extensions.Options;

namespace LucaHome.Services
{
    public class CommentService : ICommentService
    {
       public readonly ICommentRepository _commentRepository;

       public CommentService(ICommentFactory commentFactory, IOptions<DBSettings> dbSettings)
        {
            _commentRepository = commentFactory.GetRepository(dbSettings.Value.DbProvider);
        }

        public async Task<Comment> GetComment(string id) => await _commentRepository.GetByIdAsync(id);
        public async Task<List<Comment>> GetComments() => await _commentRepository.GetAllAsync();
        public async Task CreateComment(Comment comment) => await _commentRepository.CreateAsync(comment);
    }
}
