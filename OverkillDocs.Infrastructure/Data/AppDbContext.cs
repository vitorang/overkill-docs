using Microsoft.EntityFrameworkCore;
using OverkillDocs.Core.Entities.Document;
using OverkillDocs.Core.Entities.Identity;

namespace OverkillDocs.Infrastructure.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Document> Documents { get; set; }
    public DbSet<DocumentFragment> DocumentFragments { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserSession> UserSessions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.CreatedBy)
                  .WithMany()
                  .HasForeignKey(e => e.CreatedById)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<DocumentFragment>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.Document)
                  .WithMany(e => e.Fragments)
                  .HasForeignKey(e => e.DocumentId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.EditedBy)
                  .WithMany()
                  .HasForeignKey(e => e.EditedById)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Username)
                .IsRequired()
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.HasIndex(e => e.Username)
                .IsUnique()
                .HasDatabaseName("IX_User_Username"); ;
        });

        modelBuilder.Entity<UserSession>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Token)
                .HasMaxLength(26)
                .IsFixedLength()
                .IsRequired();

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.Token)
                .IsUnique()
                .HasDatabaseName("IX_UserSession_Token");
        });

        base.OnModelCreating(modelBuilder);
    }
}
