using LucaHome.Models;

namespace LucaHome.Repositories
{
    public interface ICommentRepository
    {
        public Task<Comment> GetByIdAsync(string id);
        public Task<List<Comment>> GetAllAsync();
        public Task CreateAsync(Comment comment);
    }
}
