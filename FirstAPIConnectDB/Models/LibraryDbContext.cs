using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace FirstAPIConnectDB.Models;

public partial class LibraryDbContext : DbContext
{
    public LibraryDbContext()
    {
    }

    public LibraryDbContext(DbContextOptions<LibraryDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Author> Authors { get; set; }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<BorrowRecord> BorrowRecords { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(local);user=sa;password=12345;Database=LibraryDB;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(e => e.AuthorId).HasName("PK__Authors__70DAFC34250FE424");

            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.BookId).HasName("PK__Books__3DE0C207FC1D31B1");

            entity.HasIndex(e => e.Isbn, "UQ__Books__447D36EAC26B4E92").IsUnique();

            entity.Property(e => e.Isbn)
                .HasMaxLength(20)
                .HasColumnName("ISBN");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Available");
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.Author).WithMany(p => p.Books)
                .HasForeignKey(d => d.AuthorId)
                .HasConstraintName("FK__Books__AuthorId__286302EC");
        });

        modelBuilder.Entity<BorrowRecord>(entity =>
        {
            entity.HasKey(e => e.BorrowId).HasName("PK__BorrowRe__4295F83F3C14A038");

            entity.HasOne(d => d.Book).WithMany(p => p.BorrowRecords)
                .HasForeignKey(d => d.BookId)
                .HasConstraintName("FK__BorrowRec__BookI__300424B4");

            entity.HasOne(d => d.User).WithMany(p => p.BorrowRecords)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__BorrowRec__UserI__2F10007B");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C10EEAC21");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E41E0E9B9C").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534FC6A5D65").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FullName).HasMaxLength(255);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Username).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
