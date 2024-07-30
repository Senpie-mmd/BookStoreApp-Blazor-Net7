using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApp.API.Data;

public partial class BookStoreAppDbContext : IdentityDbContext<ApiUser>
{
    public BookStoreAppDbContext()
    {
    }

    public BookStoreAppDbContext(DbContextOptions<BookStoreAppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Author> Authors { get; set; }

    public virtual DbSet<Book> Books { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Authors__3214EC0721F62AD8");

            entity.Property(e => e.Bio).HasMaxLength(250);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Books__3214EC072619CAC8");

            entity.HasIndex(e => e.Isbn, "UQ__Books__447D36EA7FE895BD").IsUnique();

            entity.Property(e => e.Image).HasMaxLength(50);
            entity.Property(e => e.Isbn)
                .HasMaxLength(50)
                .HasColumnName("ISBN");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Summary).HasMaxLength(250);
            entity.Property(e => e.Title).HasMaxLength(50);

            entity.HasOne(d => d.Author).WithMany(p => p.Books)
                .HasForeignKey(d => d.AuthorId)
                .HasConstraintName("FK_Books_ToTable");
        });

        modelBuilder.Entity<IdentityRole>().HasData(
            new IdentityRole()
            {
                Id = "04ed73f2-01f4-4078-bbb7-418f7379d7f4",
                Name = "User",
                NormalizedName = "USER"
            },
            new IdentityRole()
            {
                Id = "8783d9cb-c0b5-4f86-b6b1-2a3bfc1ac1e9",
                Name = "Admin",
                NormalizedName = "ADMIN"
            }
        );

        var hasher = new PasswordHasher<ApiUser>();

        modelBuilder.Entity<ApiUser>().HasData(
            new ApiUser()
            {
                Id = "ad990dc0-2f66-4176-95da-ebe21cadd23c",
                Email = "admin@gmail.com",
                NormalizedEmail = "ADMIN@GMAIL.COM",
                UserName = "admin@gmail.com",
                NormalizedUserName = "ADMIN@GMAIL.COM",
                PasswordHash = hasher.HashPassword(null, "P@ssword1"),
                FirstName = "system",
                LastName = "Admin"
            },
            new ApiUser()
            {
                Id = "a413b2a7-f233-4125-9574-8bbf852ab508",
                Email = "user@gmail.com",
                NormalizedEmail = "USER@GMAIL.COM",
                UserName = "user@gmail.com",
                NormalizedUserName = "USER@GMAIL.COM",
                PasswordHash = hasher.HashPassword(null, "P@ssword1"),
                FirstName = "system",
                LastName = "User"
            }
            );

        modelBuilder.Entity<IdentityUserRole<string>>().HasData(
           new IdentityUserRole<string>()
           {
               RoleId = "04ed73f2-01f4-4078-bbb7-418f7379d7f4",
               UserId = "a413b2a7-f233-4125-9574-8bbf852ab508"
           },
            new IdentityUserRole<string>()
            {
                RoleId = "8783d9cb-c0b5-4f86-b6b1-2a3bfc1ac1e9",
                UserId = "ad990dc0-2f66-4176-95da-ebe21cadd23c"
            }
           );

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
