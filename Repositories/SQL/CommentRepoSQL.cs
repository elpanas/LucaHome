using LucaHome.DBs;
using LucaHome.Models;
using Microsoft.EntityFrameworkCore;

namespace LucaHome.Repositories.SQL
{
    public class CommentRepoSQL : ICommentRepository
    {
        private readonly SQLDBContext _context;

        public CommentRepoSQL(SQLDBContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(Comment comment) => 
            await _context.Comments.AddAsync(comment);

        public async Task<List<Comment>> GetAllAsync() => 
            await _context.Comments.ToListAsync();

        public async Task<Comment> GetByIdAsync(string id) =>     
            await _context.Comments.FindAsync(id);        
    }
}
