using Application.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace Application.Entities
{
    public class Achievement
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public string? IconUrl { get; set; }

        public DateTime? AchievedAt { get; set; } = DateTime.UtcNow;

        public Guid? UserId { get; set; }
        public User User { get; set; }
    }

}
