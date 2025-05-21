using Platform_Learning_Test.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Platform_Learning_Test.Data.Factory
{
    public class ApplicationContext : IdentityDbContext<User, Role, int>
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }


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

            foreach (var entity in builder.Model.GetEntityTypes())
            {
                entity.SetTableName(entity.GetTableName().ToLowerInvariant());

                foreach (var property in entity.GetProperties())
                {
                    property.SetColumnName(property.GetColumnName().ToLowerInvariant());
                }
            }
        }
    }
}