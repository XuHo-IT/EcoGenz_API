using Application.Entities.Base;
using Application.Entities.DTOs.User;
using Application.Interface.IRepositories;
using Application.Interface.IServices;
using Application.Response;
using Google.Apis.Auth;
using System.Net;

namespace EcoGreen.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly ITokenService _tokenService;

        public AuthService(IAuthRepository authRepository, ITokenService tokenService)
        {
            _authRepository = authRepository;
            _tokenService = tokenService;
        }

        public async Task<APIResponse> LoginAsync(UserLoginDTO model)
        {
            var response = new APIResponse();

            var user = await _authRepository.FindByUserNameAsync(model.Email);
            if (user == null) user = await _authRepository.FindByEmailAsync(model.Email!);


            if (user == null || !await _authRepository.CheckPasswordAsync(user, model.Password))
            {
                response.StatusCode = HttpStatusCode.Unauthorized;
                response.isSuccess = false;
                response.ErrorMessages.Add("Invalid username or password");
                return response;
            }
            else
            {

                var roles = await _authRepository.GetRolesAsync(user);
                if (roles == null || !roles.Any())
                {
                    roles = new List<string> { "User" }; // Default role if none assigned
                }

                var token = _tokenService.GenerateJwtToken(user, roles.ToList());

                response.StatusCode = HttpStatusCode.OK;
                response.isSuccess = true;
                response.Result = new AuthResponse { Token = token, UserId = user.Id, UserName = user.UserName, Email = user.Email, ProfilePhotoUrl = user.ProfilePhotoUrl };
                return response;
            }

        }

        public async Task<APIResponse> FindUserById(string id)
        {
            var response = new APIResponse();

            var user = await _authRepository.FindByIdAsync(id);
            if (user == null)
            {
                response.StatusCode = HttpStatusCode.Unauthorized;
                response.isSuccess = false;
                response.ErrorMessages.Add("Invalid username or password");
                return response;
            }
            else
            {
                response.StatusCode = HttpStatusCode.OK;
                response.isSuccess = true;
                response.Result = user;
                return response;
            }

        }

        public async Task<APIResponse> RegisterAsync(UserRegisterDTO model, string PhotoUrl)
        {
            var response = new APIResponse();

            // Kiểm tra định dạng email trước
            if (string.IsNullOrWhiteSpace(model.Email) || !model.Email.EndsWith("@gmail.com", StringComparison.OrdinalIgnoreCase))
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                response.isSuccess = false;
                response.ErrorMessages.Add("Email must be a valid Gmail address (end with @gmail.com)");
                return response;
            }

            if (await _authRepository.FindByUserNameAsync(model.name) != null)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                response.isSuccess = false;
                response.ErrorMessages.Add("Username already exists");
                return response;
            }

            if (await _authRepository.FindByEmailAsync(model.Email) != null)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                response.isSuccess = false;
                response.ErrorMessages.Add("Email already exists");
                return response;
            }

            var user = new User
            {
                UserName = model.name,
                Email = model.Email,
                ProfilePhotoUrl = PhotoUrl,
            };

            var identityResult = await _authRepository.CreateUserAsync(user, model.Password);

            if (!identityResult.Succeeded)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                response.isSuccess = false;
                response.ErrorMessages.AddRange(identityResult.Errors.Select(e => e.Description));
                return response;
            }

            var roleResult = await _authRepository.AddRolesAsync(user, new string[] { "User" });

            if (!roleResult.Succeeded)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                response.isSuccess = false;
                response.ErrorMessages.AddRange(roleResult.Errors.Select(e => e.Description));
                return response;
            }

            response.StatusCode = HttpStatusCode.OK;
            response.isSuccess = true;
            response.Result = "User registered successfully";
            return response;
        }

        public async Task<APIResponse> GoogleLoginAsync(GoogleJsonWebSignature.Payload payload)
        {
            var response = new APIResponse();
            if (payload == null)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                response.isSuccess = false;
                response.ErrorMessages.Add("Invalid Google token");
                return response;
            }
            var user = await _authRepository.FindByEmailAsync(payload.Email);
            if (user == null)
            {
                user = new User
                {
                    UserName = payload.Name,
                    Email = payload.Email,
                    ProfilePhotoUrl = payload.Picture
                };
                var identityResult = await _authRepository.CreateAsync(user);
                if (!identityResult.Succeeded)
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.isSuccess = false;
                    response.ErrorMessages.AddRange(identityResult.Errors.Select(e => e.Description));
                    return response;
                }

                var roleResult = await _authRepository.AddRolesAsync(user, new string[] { "User" });

                if (!roleResult.Succeeded)
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.isSuccess = false;
                    response.ErrorMessages.AddRange(roleResult.Errors.Select(e => e.Description));
                    return response;
                }
            }
            var roles = await _authRepository.GetRolesAsync(user);
            if (roles == null || !roles.Any())
            {
                roles = new List<string> { "User" }; // Default role if none assigned
            }
            var token = _tokenService.GenerateJwtToken(user, roles.ToList());
            response.StatusCode = HttpStatusCode.OK;
            response.isSuccess = true;
            response.Result = new AuthResponse { Token = token, UserId = user.Id, UserName = user.UserName, Email = user.Email, ProfilePhotoUrl = user.ProfilePhotoUrl };
            return response;
        }
    }
}
