using System.ComponentModel.DataAnnotations;

namespace Application.Request.Post
{
    public class CreatePostRequest
    {
        [Required(ErrorMessage = "Content is required.")]
        public string Content { get; set; }

        [Required]
        public Guid UserId { get; set; }

    }
}
