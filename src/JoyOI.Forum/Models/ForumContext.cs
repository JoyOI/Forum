using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Pomelo.AspNetCore.Extensions.BlobStorage.Models;

namespace JoyOI.Forum.Models
{
    public class ForumContext : IdentityDbContext<User, Role, Guid>, IBlobStorageDbContext
    {
        public ForumContext(DbContextOptions opt) : base(opt)
        {
        }

        public DbSet<Forum> Forums { get; set; }

        public DbSet<Thread> Threads { get; set; }

        public DbSet<Post> Posts { get; set; }

        public DbSet<Blob> Blobs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.SetupBlobStorage();

            builder.Entity<Thread>(e =>
            {
                e.HasIndex(x => x.IsTop);
                e.HasIndex(x => x.CreationTime);
                e.HasIndex(x => x.LastReplyTime);
                e.HasIndex(x => x.IsAnnouncement);
            });

            builder.Entity<Post>(e =>
            {
                e.HasIndex(x => x.Time);
            });

            builder.Entity<Forum>(e =>
            {
                e.Property(x => x.ParentId).IsRequired(false);
                e.HasIndex(x => x.PRI);
                e.HasIndex(x => x.IsHidden);
            });
        }
    }
}
