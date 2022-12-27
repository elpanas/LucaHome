using ProvaRest.Models;

namespace LucaHome.Services
{
    public interface ICommentService
    {
        Task<Comment> GetComment(string id);
        Task<List<Comment>> GetComments();
        Task CreateComment(Comment comment);
        Task<bool> CommentExists(string id);
    }
}
