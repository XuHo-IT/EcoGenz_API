using Application.Entities.Base;
using Application.Entities.DTOs.User;
using Application.Interface.IServices;
using Application.Request.User;
using AutoMapper;
using EcoGreen.Service;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;

namespace EcoGreen.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        public AuthController(IAuthService authService, IMapper mapper)
        {
            _authService = authService;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] UserRegisterDTO registerModel, IFormFile? imageFile,
            [FromServices] CloudinaryService cloudinaryService)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = _mapper.Map<User>(registerModel);
            if (imageFile != null && imageFile.Length > 0)
            {
                var imageUrl = await cloudinaryService.UploadImageAsync(imageFile);
                user.ProfilePhotoUrl = imageUrl;
            }
            var response = await _authService.RegisterAsync(registerModel, user.ProfilePhotoUrl);
            if (response.isSuccess)
            {
                return Ok(response);
            }
            return StatusCode((int)response.StatusCode, response);
        }

        // POST: /api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO loginRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _authService.LoginAsync(loginRequest);
            if (response.isSuccess)
            {
                return Ok(response);
            }
            return StatusCode((int)response.StatusCode, response);
        }
        [HttpPut("update-user")]
        public async Task<IActionResult> UpdateUser(
        [FromForm] UserUpdateRequest updateRequest,
        IFormFile? imageFile,
        [FromServices] CloudinaryService cloudinaryService)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // ✅ Upload image if available and assign to request
            if (imageFile != null && imageFile.Length > 0)
            {
                var imageUrl = await cloudinaryService.UploadImageAsync(imageFile);
                updateRequest.ProfilePhotoUrl = imageUrl;
            }

            // ✅ Call the service with the updated request
            var response = await _authService.UpdateUserAsync(updateRequest);

            return StatusCode((int)response.StatusCode, response);
        }


        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword(
            [FromBody] UserUpdateRequest updateRequest,
            [FromQuery] string currentPassword,
            [FromQuery] string newPassword)
        {
            var response = await _authService.ChangeUserPasswordAsync(updateRequest, currentPassword, newPassword);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("userWithPoint")]
        public async Task<IActionResult> GetUsersByPointAscAsync()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _authService.GetUsersByPointAscAsync();
            if (response.isSuccess)
            {
                return Ok(response);
            }
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPost("google")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            var payload = await VerifyGoogleTokenAsync(request.tokenId);
            if (payload == null)
            {
                return BadRequest("Invalid Google token");
            }
            var response = await _authService.GoogleLoginAsync(payload, request.role);
            if (response.isSuccess)
            {
                return Ok(response);
            }
            return StatusCode((int)response.StatusCode, response);

        }

        private async Task<GoogleJsonWebSignature.Payload> VerifyGoogleTokenAsync(string token)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings();
            return await GoogleJsonWebSignature.ValidateAsync(token, settings);
        }
        [HttpGet("get-user/{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _authService.FindUserById(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

    }
}
