using Microsoft.EntityFrameworkCore;
using OverkillDocs.Core.Entities;

namespace OverkillDocs.Infrastructure.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Document> Documents { get; set; }
        public DbSet<DocumentFragment> DocumentFragments { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Document>(entity =>
            {
                entity.HasKey(d => d.Id);

                entity.HasOne(d => d.CreatedBy)
                      .WithMany()
                      .HasForeignKey(d => d.CreatedById)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<DocumentFragment>(entity =>
            {
                entity.HasKey(f => f.Id);

                entity.HasOne(f => f.Document)
                      .WithMany(d => d.Fragments)
                      .HasForeignKey(f => f.DocumentId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(f => f.EditedBy)
                      .WithMany()
                      .HasForeignKey(f => f.EditedById)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.HasIndex(u => u.Username).IsUnique();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
