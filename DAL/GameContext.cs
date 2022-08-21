using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace DAL
{
    public partial class GameContext : DbContext
    {
        public GameContext()
        {
        }

        public GameContext(DbContextOptions<GameContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Game> Games { get; set; }
        public virtual DbSet<Session> Sessions { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=Game;Username=postgres;Password=password;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Russian_Russia.1251");

            modelBuilder.Entity<Game>(entity =>
            {
                entity.ToTable("Game");

                entity.Property(e => e.Id).UseIdentityAlwaysColumn();

                entity.Property(e => e.Data).HasColumnType("character varying");

                entity.HasOne(d => d.HostNavigation)
                    .WithMany(p => p.Games)
                    .HasForeignKey(d => d.Host)
                    .HasConstraintName("host");
            });

            modelBuilder.Entity<Session>(entity =>
            {
                entity.ToTable("Session");

                entity.Property(e => e.Id).UseIdentityAlwaysColumn();

                entity.HasOne(d => d.GameNavigation)
                    .WithMany(p => p.Sessions)
                    .HasForeignKey(d => d.Game)
                    .HasConstraintName("Game");

                entity.HasOne(d => d.UserNavigation)
                    .WithMany(p => p.Sessions)
                    .HasForeignKey(d => d.User)
                    .HasConstraintName("User");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.Property(e => e.Id).UseIdentityAlwaysColumn();

                entity.Property(e => e.Name).HasColumnType("character varying");

                entity.Property(e => e.Password).HasColumnType("character varying");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
