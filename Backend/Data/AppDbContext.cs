using Microsoft.EntityFrameworkCore;
using TrainingHelpDeskApi.Models;

namespace TrainingHelpDeskApi.Data
{
    // EF Core DbContext used to persist trainee chat history to SQL Server.
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<ChatLog> ChatLogs => Set<ChatLog>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChatLog>(entity =>
            {
                entity.ToTable("ChatLogs");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Question).IsRequired().HasMaxLength(1000);
                entity.Property(x => x.Answer).IsRequired();
                entity.Property(x => x.SourcesUsed).HasMaxLength(500);
                entity.Property(x => x.CreatedAtUtc).IsRequired();
            });
        }
    }
}
