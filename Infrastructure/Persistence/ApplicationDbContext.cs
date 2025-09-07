using Microsoft.EntityFrameworkCore;
using CompatibleAPI.Domain.Entities;

namespace CompatibleAPI.Infrastructure.Persistence
{
    /// <summary>
    /// Database context for the application
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the ApplicationDbContext
        /// </summary>
        /// <param name="options">The options for this context</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Gets or sets the profiles in the database
        /// </summary>
        public DbSet<Profile> Profiles { get; set; } = null!;

        /// <summary>
        /// Gets or sets the photos in the database
        /// </summary>
        public DbSet<Photo> Photos { get; set; } = null!;

        /// <summary>
        /// Gets or sets the matches in the database
        /// </summary>
        public DbSet<Match> Matches { get; set; } = null!;

        /// <summary>
        /// Gets or sets the messages in the database
        /// </summary>
        public DbSet<Message> Messages { get; set; } = null!;

        /// <summary>
        /// Configures the database model
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Profile entity
            modelBuilder.Entity<Profile>(entity =>
            {
                entity.ToTable("profiles");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("gen_random_uuid()");
                entity.Property(e => e.Name).HasColumnName("name").IsRequired().HasMaxLength(100);
                entity.Property(e => e.Age).HasColumnName("age");
                entity.Property(e => e.Bio).HasColumnName("bio").IsRequired().HasMaxLength(500);
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
                entity.Property(e => e.RetiredAt).HasColumnName("retired_at");
            });

            // Configure Photo entity
            modelBuilder.Entity<Photo>(entity =>
            {
                entity.ToTable("photos");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("gen_random_uuid()");
                entity.Property(e => e.Url).HasColumnName("url").IsRequired();
                entity.Property(e => e.ProfileId).HasColumnName("profile_id");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
                entity.Property(e => e.RetiredAt).HasColumnName("retired_at");

                entity.HasOne(e => e.Profile)
                    .WithMany(p => p.Photos)
                    .HasForeignKey(e => e.ProfileId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Match entity
            modelBuilder.Entity<Match>(entity =>
            {
                entity.ToTable("matches");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("gen_random_uuid()");
                entity.Property(e => e.ProfileId).HasColumnName("profile_id");
                entity.Property(e => e.MatchedProfileId).HasColumnName("matched_profile_id");
                entity.Property(e => e.IsAccepted).HasColumnName("is_accepted");
                entity.Property(e => e.IsRejected).HasColumnName("is_rejected");
                entity.Property(e => e.MatchedAt).HasColumnName("matched_at");
                entity.Property(e => e.Compatibility).HasColumnName("compatibility");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
                entity.Property(e => e.RetiredAt).HasColumnName("retired_at");

                entity.HasOne(e => e.Profile)
                    .WithMany(p => p.Matches)
                    .HasForeignKey(e => e.ProfileId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.MatchedProfile)
                    .WithMany(p => p.MatchedBy)
                    .HasForeignKey(e => e.MatchedProfileId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Message entity
            modelBuilder.Entity<Message>(entity =>
            {
                entity.ToTable("messages");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("gen_random_uuid()");
                entity.Property(e => e.MatchId).HasColumnName("match_id");
                entity.Property(e => e.SenderProfileId).HasColumnName("sender_profile_id");
                entity.Property(e => e.Content).HasColumnName("content").IsRequired().HasMaxLength(1000);
                entity.Property(e => e.IsRead).HasColumnName("is_read");
                entity.Property(e => e.ReadAt).HasColumnName("read_at");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
                entity.Property(e => e.RetiredAt).HasColumnName("retired_at");

                entity.HasOne(e => e.Match)
                    .WithMany()
                    .HasForeignKey(e => e.MatchId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.SenderProfile)
                    .WithMany()
                    .HasForeignKey(e => e.SenderProfileId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        /// <summary>
        /// Saves all changes made in this context to the database
        /// </summary>
        /// <returns>The number of state entries written to the database</returns>
        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        /// <summary>
        /// Saves all changes made in this context to the database asynchronously
        /// </summary>
        /// <returns>A task that represents the asynchronous save operation</returns>
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Updates the timestamps for entities before saving changes
        /// </summary>
        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is BaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                var entity = (BaseEntity)entry.Entity;

                if (entry.State == EntityState.Added)
                {
                    entity.CreatedAt = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entity.UpdatedAt = DateTime.UtcNow;
                }
            }
        }
    }
} 