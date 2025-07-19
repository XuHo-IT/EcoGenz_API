namespace Application.Request.User
{
    public class UserUpdateRequest
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string? ProfilePhotoUrl { get; set; }
    }
}
