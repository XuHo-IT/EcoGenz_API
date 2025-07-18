using Application.Entities.Base.Post;
using Application.Interface.IRepositories;
using InfrasStructure.EntityFramework.Data;
using Microsoft.EntityFrameworkCore;

namespace InfrasStructure.EntityFramework.Repository
{
    public class CommentRepository : ICommentRepository
    {
        private readonly ApplicationDBContext _context;

        public CommentRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task AddCommentAsync(Comment comment)
        {
            await _context.Comments.AddAsync(comment);
        }
        public async Task<List<Comment>> ListComment()
        {
            return await _context.Comments.ToListAsync();
        }
        public async Task<List<Comment>> GetCommentByActivityId(Guid activityId)
        {
            return await _context.Comments
                                 .Where(c => c.ActivityId == activityId)
                                 .Include(c => c.User)
                                 .ToListAsync();
        }


        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }

}
