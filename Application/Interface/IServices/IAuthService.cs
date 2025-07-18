using Application.Entities.DTOs.User;
using Application.Response;
using Google.Apis.Auth;

namespace Application.Interface.IServices
{
    public interface IAuthService
    {
        Task<APIResponse> RegisterAsync(UserRegisterDTO model, string PhotoUrl);
        Task<APIResponse> LoginAsync(UserLoginDTO model);
        Task<APIResponse> GoogleLoginAsync(GoogleJsonWebSignature.Payload payload);
        Task<APIResponse> FindUserById(string id);
        Task<APIResponse> GetUsersByPointAscAsync();

    }
}
