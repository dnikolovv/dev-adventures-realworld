using Conduit.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Conduit.Data.EntityFramework
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base (options)
        {
        }

        public DbSet<Article> Articles { get; set; }

        public DbSet<ArticleFavorite> ArticleFavorites { get; set; }

        public DbSet<ArticleTag> ArticleTags { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<FollowedUser> FollowedUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ArticleFavorite>(b =>
            {
                b.HasKey(af => new { af.ArticleId, af.UserId });

                b.HasOne(af => af.User)
                    .WithMany(u => u.Favorites)
                    .HasForeignKey(af => af.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<ArticleTag>(b =>
            {
                b.HasKey(at => new { at.ArticleId, at.TagId });
            });

            builder.Entity<FollowedUser>(b =>
            {
                b.HasKey(f => new { f.FollowerId, f.FollowingId });

                b.HasOne(f => f.Follower)
                    .WithMany(u => u.Following)
                    .HasForeignKey(f => f.FollowerId)
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(f => f.Following)
                    .WithMany(u => u.Followers)
                    .HasForeignKey(f => f.FollowingId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            base.OnModelCreating(builder);
        }
    }
}
