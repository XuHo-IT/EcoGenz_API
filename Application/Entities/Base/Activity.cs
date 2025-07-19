using Application.Entities.Base.Post;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Application.Entities.Base
{
    public class Activity
    {
        [Key]
        public Guid ActivityId { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, ErrorMessage = "Title can't exceed 100 characters.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(1000, ErrorMessage = "Description can't exceed 1000 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Location is required.")]
        [StringLength(200, ErrorMessage = "Location can't exceed 200 characters.")]

        public string Location { get; set; }
        [Required(ErrorMessage = "Amount of people is required.")]
        public int AmountOfPeople { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        public DateTime Date { get; set; }

        [StringLength(500, ErrorMessage = "Media URL can't exceed 500 characters.")]
        public string? MediaUrl { get; set; }

        [Required]
        public Guid CreatedByCompanyId { get; set; }

        [ForeignKey(nameof(CreatedByCompanyId))]
        public virtual User CompanyUser { get; set; }

        public bool IsApproved { get; set; }

        public virtual ICollection<Registration> ActivityRegistrations { get; set; } = new List<Registration>();
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    }
}
