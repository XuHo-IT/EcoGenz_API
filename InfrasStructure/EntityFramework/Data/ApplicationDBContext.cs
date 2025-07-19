using Application.Entities;
using Application.Entities.Base;
using Application.Entities.Base.Post;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InfrasStructure.EntityFramework.Data
{
    public class ApplicationDBContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
            : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<Registration> Registrations { get; set; }
        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Share> Shares { get; set; }
        public DbSet<Achievement> Achievements { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Achievement>()
      .HasOne(r => r.User)
      .WithMany(u => u.Achievements)
      .HasForeignKey(r => r.UserId)
      .OnDelete(DeleteBehavior.Restrict);


            // User has many Registrations
            modelBuilder.Entity<Registration>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Registration>()
                .HasOne(r => r.Activity)
                .WithMany()
                .HasForeignKey(r => r.ActivityId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Certificate>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Certificate>()
                .HasOne(c => c.Activity)
                .WithMany()
                .HasForeignKey(c => c.ActivityId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Certificate>()
                .HasOne(c => c.CompanyUser)
                .WithMany()
                .HasForeignKey(c => c.IssuedByCompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Activity>()
                .HasOne(a => a.CompanyUser)
                .WithMany()
                .HasForeignKey(a => a.CreatedByCompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Post>()
     .HasOne(p => p.User)
     .WithMany(u => u.Posts)
     .HasForeignKey(p => p.UserId)
     .OnDelete(DeleteBehavior.Cascade);



            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Activity)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.ActivityId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Like>()
                .HasOne(l => l.User)
                .WithMany(u => u.Likes)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Restrict);



            modelBuilder.Entity<Like>()
                .HasIndex(l => new { l.UserId, l.PostId })
                .IsUnique();


            modelBuilder.Entity<Share>()
                .HasOne(s => s.User)
                .WithMany(u => u.Shares)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);



            modelBuilder.Entity<Share>()
                .HasIndex(s => new { s.UserId, s.PostId })
                .IsUnique();



            // Optional: Add a unique constraint to prevent duplicate registrations
            modelBuilder.Entity<Registration>()
                .HasIndex(r => new { r.UserId, r.ActivityId })
                .IsUnique();

        }
    }
}
