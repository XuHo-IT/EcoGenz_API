using Application.Entities.Base.Post;

namespace Application.Interface.IRepositories
{
    public interface ICommentRepository
    {
        Task AddCommentAsync(Comment comment);
        Task SaveChangesAsync();
        Task<List<Comment>> ListComment();
        Task<List<Comment>> GetCommentByActivityId(Guid activityId);
    }

}
