using Platform_Learning_Test.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;

namespace Platform_Learning_Test.Data.Factory
{
    public class ApplicationContext : IdentityDbContext<User, Role, int>
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<AnswerOption> AnswerOptions { get; set; }
        public DbSet<UserAnswer> UserAnswers { get; set; }
        public DbSet<TestReview> TestReviews { get; set; }
        public DbSet<UserTestHistory> UserTestHistories { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options) { }
        public ApplicationContext() : base()
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
                    ?? "Server=(localdb)\\mssqllocaldb;Database=Platform_Learning_Test;Trusted_Connection=True;TrustServerCertificate=True;";

                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            ConfigureTestSystemEntities(builder);
           

            builder.Entity<User>(entity =>
            {

                entity.HasIndex(u => u.NormalizedEmail)
                    .IsUnique()
                    .HasDatabaseName("IX_Users_NormalizedEmail")
                    .HasFilter(null);

                entity.HasIndex(u => u.NormalizedUserName)
                    .IsUnique()
                    .HasDatabaseName("IX_Users_NormalizedUserName")
                    .HasFilter(null);

                entity.Property(u => u.Email)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(u => u.NormalizedEmail)
                    .IsRequired()
                    .HasMaxLength(256)
                    .HasComputedColumnSql("UPPER([Email])", stored: true);

                entity.Property(u => u.UserName)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(u => u.NormalizedUserName)
                    .IsRequired()
                    .HasMaxLength(256)
                    .HasComputedColumnSql("UPPER([UserName])", stored: true);

                entity.Property(u => u.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(u => u.PasswordHash)
                    .IsRequired();

                entity.Property(u => u.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.HasMany(u => u.UserRoles)
                    .WithOne(ur => ur.User)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<Role>(entity =>
            {

                entity.HasIndex(r => r.NormalizedName)
                    .IsUnique()
                    .HasDatabaseName("IX_Roles_NormalizedName")
                    .HasFilter(null);

                entity.Property(r => r.Name)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(r => r.NormalizedName)
                    .IsRequired()
                    .HasMaxLength(256)
                    .HasComputedColumnSql("UPPER([Name])", stored: true);

                entity.Property(r => r.Description)
                    .HasMaxLength(200)
                    .HasDefaultValue(string.Empty);

                entity.HasMany(r => r.UserRoles)
                    .WithOne(ur => ur.Role)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<UserRole>(entity =>
            {
                entity.HasKey(ur => new { ur.UserId, ur.RoleId });

                entity.HasOne(ur => ur.User)
                    .WithMany(u => u.UserRoles)
                    .HasForeignKey(ur => ur.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

        }
             private void ConfigureTestSystemEntities(ModelBuilder builder)
        {
            builder.Entity<Test>(entity =>
            {
                entity.ToTable("Tests"); 
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Title).IsRequired().HasMaxLength(200);
                entity.Property(t => t.Description).HasMaxLength(1000);
                entity.Property(t => t.Category).HasMaxLength(100);
                entity.Property(t => t.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                
                entity.Property(t => t.Difficulty)
               .HasConversion<string>(); 

                entity.HasMany(t => t.Questions)
                    .WithOne(q => q.Test)
                    .HasForeignKey(q => q.TestId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            builder.Entity<Question>(entity =>
            {
                entity.Property(q => q.Text).IsRequired().HasMaxLength(1000);
                entity.Property(q => q.TimeLimitSeconds).HasDefaultValue(60);

                entity.HasMany(q => q.AnswerOptions)
                    .WithOne(a => a.Question)
                    .HasForeignKey(a => a.QuestionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<AnswerOption>(entity =>
            {
                entity.Property(a => a.Text).IsRequired().HasMaxLength(500);
            });

            builder.Entity<UserTestHistory>(entity =>
            {
                entity.Property(h => h.StartedAt).HasDefaultValueSql("GETUTCDATE()");

                entity.HasMany(h => h.UserAnswers)
                    .WithOne(a => a.UserTestHistory)
                    .HasForeignKey(a => a.UserTestHistoryId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<TestReview>(entity =>
            {
                entity.Property(r => r.Text).IsRequired().HasMaxLength(2000);
                entity.Property(r => r.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });
        }

        
    }
}
