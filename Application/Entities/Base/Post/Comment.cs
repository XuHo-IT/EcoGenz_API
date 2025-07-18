using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Application.Entities.Base.Post
{
    public class Comment
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Comment text is required.")]
        [StringLength(500, ErrorMessage = "Comment can't exceed 500 characters.")]
        public string Text { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid ActivityId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        [ForeignKey(nameof(ActivityId))]
        public virtual Activity Activity { get; set; }
    }
}
