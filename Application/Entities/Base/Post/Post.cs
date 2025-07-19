using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Application.Entities.Base.Post
{
    public class Post
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Post content is required.")]
        public string Content { get; set; }

        [StringLength(500, ErrorMessage = "Media URL can't exceed 500 characters.")]
        public string? MediaUrl { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public Guid UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        //public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        //public virtual ICollection<Like> Likes { get; set; } = new List<Like>();
        //public virtual ICollection<Share> Shares { get; set; } = new List<Share>();
    }
}
