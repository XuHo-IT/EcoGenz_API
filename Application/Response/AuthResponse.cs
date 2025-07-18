namespace Application.Response
{
    public class AuthResponse
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public string ProfilePhotoUrl { get; set; } = string.Empty;
        public string Role { get; set; }
    }
}
