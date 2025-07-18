using System.ComponentModel.DataAnnotations;

namespace Application.Request.Post
{
    public class CommentRequest
    {
        [Required]
        public Guid ActivityId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "Content is required.")]
        [StringLength(500, ErrorMessage = "Comment can't exceed 500 characters.")]
        public string Content { get; set; } = string.Empty;
    }
}
