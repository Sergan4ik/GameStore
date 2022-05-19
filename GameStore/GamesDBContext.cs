using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace GameStore
{
    public partial class GamesDBContext : DbContext
    {
        public GamesDBContext()
        {
        }

        public GamesDBContext(DbContextOptions<GamesDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Game> Games { get; set; } = null!;
        public virtual DbSet<GameCopy> GameCopies { get; set; } = null!;
        public virtual DbSet<GameStudio> GameStudios { get; set; } = null!;
        public virtual DbSet<Item> Items { get; set; } = null!;
        public virtual DbSet<ItemsInInventory> ItemsInInventories { get; set; } = null!;
        public virtual DbSet<Promocode> Promocodes { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server= SERGIY_KLOCHAK; Database = GamesDB; Trusted_Connection = True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Game>(entity =>
            {
                entity.Property(e => e.GameId).HasColumnName("Game_Id");

                entity.Property(e => e.AgePermission).HasColumnName("Age_permission");

                entity.Property(e => e.Description).IsUnicode(false);

                entity.Property(e => e.GameStudioId).HasColumnName("Game_Studio_Id");

                entity.Property(e => e.Genre)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Price)
                    .HasColumnType("money")
                    .HasDefaultValueSql("((0))");

                entity.HasOne(d => d.GameStudio)
                    .WithMany(p => p.Games)
                    .HasForeignKey(d => d.GameStudioId)
                    .HasConstraintName("FK_Games_Game_Studios");
            });

            modelBuilder.Entity<GameCopy>(entity =>
            {
                entity.HasKey(e => e.CopyId);

                entity.ToTable("Game_copies");

                entity.Property(e => e.CopyId).HasColumnName("Copy_Id");

                entity.Property(e => e.BuyDate)
                    .HasColumnType("date")
                    .HasColumnName("Buy_date");

                entity.Property(e => e.GameId).HasColumnName("Game_Id");

                entity.Property(e => e.UserId).HasColumnName("User_Id");

                entity.HasOne(d => d.Game)
                    .WithMany(p => p.GameCopies)
                    .HasForeignKey(d => d.GameId)
                    .HasConstraintName("FK_Game_copies_Games");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.GameCopies)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Game_copies_Users");
            });

            modelBuilder.Entity<GameStudio>(entity =>
            {
                entity.ToTable("Game_Studios");

                entity.Property(e => e.Description).IsUnicode(false);

                entity.Property(e => e.StudioName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("Studio_Name");
            });

            modelBuilder.Entity<Item>(entity =>
            {
                entity.Property(e => e.Price).HasColumnType("money");

                entity.Property(e => e.Rarity)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.GameNavigation)
                    .WithMany(p => p.Items)
                    .HasForeignKey(d => d.Game)
                    .HasConstraintName("FK_Items_Games");
            });

            modelBuilder.Entity<ItemsInInventory>(entity =>
            {
                entity.HasKey(e => e.ItemCopyId);

                entity.ToTable("Items_in_inventories");

                entity.Property(e => e.ItemCopyId).HasColumnName("Item_copy_Id");

                entity.Property(e => e.OwnerId).HasColumnName("Owner_id");

                entity.Property(e => e.SourceItemId).HasColumnName("Source_item_Id");

                entity.HasOne(d => d.Owner)
                    .WithMany(p => p.ItemsInInventories)
                    .HasForeignKey(d => d.OwnerId)
                    .HasConstraintName("FK_Items_in_inventories_Users");

                entity.HasOne(d => d.SourceItem)
                    .WithMany(p => p.ItemsInInventories)
                    .HasForeignKey(d => d.SourceItemId)
                    .HasConstraintName("FK_Items_in_inventories_Items");
            });

            modelBuilder.Entity<Promocode>(entity =>
            {
                entity.HasKey(e => e.Promocode1);

                entity.Property(e => e.Promocode1)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("Promocode");

                entity.HasOne(d => d.GameNavigation)
                    .WithMany(p => p.Promocodes)
                    .HasForeignKey(d => d.Game)
                    .HasConstraintName("FK_Promocodes_Games");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Balance).HasColumnType("money");

                entity.Property(e => e.BirthDate)
                    .HasColumnType("date")
                    .HasColumnName("Birth_date");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Username)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
