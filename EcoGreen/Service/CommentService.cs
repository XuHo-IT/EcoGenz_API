using Application.Entities.Base.Post;
using Application.Interface.IRepositories;
using Application.Interface.IServices;
using Application.Response;
using System.Net;

namespace EcoGreen.Service
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;

        public CommentService(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }

        public async Task<APIResponse> AddCommentAsync(Comment comment)
        {
            var response = new APIResponse();

            try
            {
                comment.CreatedAt = DateTime.UtcNow;
                await _commentRepository.AddCommentAsync(comment);
                await _commentRepository.SaveChangesAsync();

                response.Result = comment;
                response.StatusCode = HttpStatusCode.OK;
                response.isSuccess = true;
            }
            catch (Exception ex)
            {
                response.isSuccess = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                response.ErrorMessages.Add(ex.Message);
            }

            return response;
        }
        public async Task<APIResponse> ListCommentAsync()
        {
            var response = new APIResponse();

            try
            {
                var comments = await _commentRepository.ListComment();

                response.Result = comments;
                response.StatusCode = HttpStatusCode.OK;
                response.isSuccess = true;
            }
            catch (Exception ex)
            {
                response.isSuccess = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                response.ErrorMessages.Add(ex.Message);
            }

            return response;
        }

        public async Task<APIResponse> GetCommentByActivityId(Guid activityId)
        {
            var response = new APIResponse();

            try
            {
                var comments = await _commentRepository.GetCommentByActivityId(activityId);

                response.Result = comments;
                response.StatusCode = HttpStatusCode.OK;
                response.isSuccess = true;
            }
            catch (Exception ex)
            {
                response.isSuccess = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                response.ErrorMessages.Add(ex.Message);
            }

            return response;
        }

    }
}
