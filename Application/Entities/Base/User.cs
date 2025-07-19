using Application.Entities.Base.Post;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Application.Entities.Base
{
    public class User : IdentityUser<Guid>
    {
        [Url(ErrorMessage = "Profile photo must be a valid URL.")]
        public string ProfilePhotoUrl { get; set; } = string.Empty;
        public ICollection<Application.Entities.Base.Post.Post> Posts { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Like> Likes { get; set; }
        public ICollection<Share> Shares { get; set; }
        public virtual ICollection<Registration> ActivityRegistrations { get; set; } = new List<Registration>();

        public int DoingAction { get; set; } = 0;
        public int ImpactPoints { get; set; } = 0;

        public virtual ICollection<Achievement> Achievements { get; set; } = new List<Achievement>();


    }
}
