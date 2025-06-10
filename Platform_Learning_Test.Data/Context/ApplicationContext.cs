using Platform_Learning_Test.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;

namespace Platform_Learning_Test.Data.Context
{
    public class ApplicationContext : IdentityDbContext<User, Role, int, IdentityUserClaim<int>,
    UserRole, 
    IdentityUserLogin<int>,
    IdentityRoleClaim<int>,
    IdentityUserToken<int>>
    {

        public DbSet<Test> Tests { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<AnswerOption> AnswerOptions { get; set; }
        public DbSet<UserAnswer> UserAnswers { get; set; }
        public DbSet<TestReview> TestReviews { get; set; }
        public DbSet<UserTestHistory> UserTestHistories { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options) { }
        public ApplicationContext() : base() { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
                    ?? "Server=Angelica-Val;Database=PlatformLearningTestDB;Trusted_Connection=True;TrustServerCertificate=True;";

                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            

            builder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.Property(u => u.IsActive)
                    .HasDefaultValue(true);

                entity.Property(u => u.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

            });

            builder.Entity<Role>(entity =>
            {
                entity.ToTable("Roles");
                    
            });
            builder.Entity<UserRole>(entity =>
            {
                entity.ToTable("UserRoles");
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


            ConfigureTestSystemEntities(builder);
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
            builder.Entity<UserAnswer>(entity =>
            {
               
                entity.HasOne(a => a.UserTestHistory)
                    .WithMany(h => h.UserAnswers)
                    .HasForeignKey(a => a.UserTestHistoryId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(a => a.Question)
                    .WithMany(q => q.UserAnswers)
                    .HasForeignKey(a => a.QuestionId)
                    .OnDelete(DeleteBehavior.Restrict);


                    entity.HasOne(a => a.AnswerOption)
                        .WithMany(a => a.UserAnswers)
                        .HasForeignKey(a => a.AnswerOptionId)
                        .OnDelete(DeleteBehavior.Restrict);
                
            });
        }
    } }

  
