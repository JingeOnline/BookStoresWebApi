using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace BookStoresWebApi.Models
{
    public partial class BookStoresDBContext : DbContext
    {
        public BookStoresDBContext()
        {
        }

        public BookStoresDBContext(DbContextOptions<BookStoresDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Author> Authors { get; set; }
        public virtual DbSet<Book> Books { get; set; }
        public virtual DbSet<BookAuthor> BookAuthors { get; set; }
        public virtual DbSet<Job> Jobs { get; set; }
        public virtual DbSet<Publisher> Publishers { get; set; }
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Sale> Sales { get; set; }
        public virtual DbSet<Store> Stores { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Name=BookStoresDB");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Chinese_PRC_CI_AS");

            modelBuilder.Entity<Author>(entity =>
            {
                entity.Property(e => e.Address).IsUnicode(false);

                entity.Property(e => e.City).IsUnicode(false);

                entity.Property(e => e.EmailAddress).IsUnicode(false);

                entity.Property(e => e.FirstName).IsUnicode(false);

                entity.Property(e => e.LastName).IsUnicode(false);

                entity.Property(e => e.Phone)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('UNKNOWN')")
                    .IsFixedLength(true);

                entity.Property(e => e.State)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Zip)
                    .IsUnicode(false)
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<Book>(entity =>
            {
                entity.Property(e => e.Notes).IsUnicode(false);

                entity.Property(e => e.PublishedDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Title).IsUnicode(false);

                entity.Property(e => e.Type)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('UNDECIDED')")
                    .IsFixedLength(true);

                entity.HasOne(d => d.Pub)
                    .WithMany(p => p.Books)
                    .HasForeignKey(d => d.PubId)
                    .HasConstraintName("FK__Book__pub_id__3E52440B");
            });

            modelBuilder.Entity<BookAuthor>(entity =>
            {
                entity.HasKey(e => new { e.AuthorId, e.BookId });

                entity.HasOne(d => d.Author)
                    .WithMany(p => p.BookAuthors)
                    .HasForeignKey(d => d.AuthorId)
                    .HasConstraintName("FK__BookAutho__autho__3F466844");

                entity.HasOne(d => d.Book)
                    .WithMany(p => p.BookAuthors)
                    .HasForeignKey(d => d.BookId)
                    .HasConstraintName("FK__BookAutho__book___403A8C7D");
            });

            modelBuilder.Entity<Job>(entity =>
            {
                entity.Property(e => e.JobDesc)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('New Position - title not formalized yet')");
            });

            modelBuilder.Entity<Publisher>(entity =>
            {
                entity.HasKey(e => e.PubId)
                    .HasName("PK__Publishe__2515F22287EF5BE1");

                entity.Property(e => e.City).IsUnicode(false);

                entity.Property(e => e.Country)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('USA')");

                entity.Property(e => e.PublisherName).IsUnicode(false);

                entity.Property(e => e.State)
                    .IsUnicode(false)
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.Property(e => e.Token).IsUnicode(false);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.RefreshTokens)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__RefreshTo__user___412EB0B6");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.RoleDesc)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('New Position - title not formalized yet')");
            });

            modelBuilder.Entity<Sale>(entity =>
            {
                entity.Property(e => e.OrderNum).IsUnicode(false);

                entity.Property(e => e.PayTerms).IsUnicode(false);

                entity.Property(e => e.StoreId)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.HasOne(d => d.Book)
                    .WithMany(p => p.Sales)
                    .HasForeignKey(d => d.BookId)
                    .HasConstraintName("FK__Sale__book_id__4222D4EF");

                entity.HasOne(d => d.Store)
                    .WithMany(p => p.Sales)
                    .HasForeignKey(d => d.StoreId)
                    .HasConstraintName("FK__Sale__store_id__4316F928");
            });

            modelBuilder.Entity<Store>(entity =>
            {
                entity.Property(e => e.StoreId)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.City).IsUnicode(false);

                entity.Property(e => e.State)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.StoreAddress).IsUnicode(false);

                entity.Property(e => e.StoreName).IsUnicode(false);

                entity.Property(e => e.Zip)
                    .IsUnicode(false)
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PK_user_id_2")
                    .IsClustered(false);

                entity.Property(e => e.EmailAddress).IsUnicode(false);

                entity.Property(e => e.FirstName).IsUnicode(false);

                entity.Property(e => e.HireDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.LastName).IsUnicode(false);

                entity.Property(e => e.MiddleName)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Password).IsUnicode(false);

                entity.Property(e => e.PubId).HasDefaultValueSql("((1))");

                entity.Property(e => e.RoleId).HasDefaultValueSql("((1))");

                entity.Property(e => e.Source).IsUnicode(false);

                entity.HasOne(d => d.Pub)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.PubId)
                    .HasConstraintName("FK__User__pub_id__44FF419A");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK__User__role_id__440B1D61");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
