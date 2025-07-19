using Application.Entities.DTOs.User;
using Application.Request.User;
using Application.Response;
using Google.Apis.Auth;

namespace Application.Interface.IServices
{
    public interface IAuthService
    {
        Task<APIResponse> RegisterAsync(UserRegisterDTO model, string PhotoUrl);
        Task<APIResponse> LoginAsync(UserLoginDTO model);
        Task<APIResponse> GoogleLoginAsync(GoogleJsonWebSignature.Payload payload, string role);
        Task<APIResponse> FindUserById(string id);
        Task<APIResponse> GetUsersByPointAscAsync();
        Task<APIResponse> UpdateUserAsync(UserUpdateRequest updateRequest);
        Task<APIResponse> ChangeUserPasswordAsync(UserUpdateRequest updateRequest, string currentPassword, string newPassword);

    }
}
