using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Compago.Data;

public partial class CompagoDbContext : DbContext
{
    public CompagoDbContext(DbContextOptions<CompagoDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<InvoiceTag> InvoiceTags { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<InvoiceTag>(entity =>
        {
            entity.HasKey(e => new { e.InvoiceId, e.TagId }).HasName("InvoiceTag_PK");

            entity.ToTable("InvoiceTag");

            entity.Property(e => e.InvoiceId).HasMaxLength(100);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.Tag).WithMany(p => p.InvoiceTags)
                .HasForeignKey(d => d.TagId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("InvoiceTag_TagFK");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Role_PK");

            entity.ToTable("Role");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Tag_PK");

            entity.ToTable("Tag");

            entity.Property(e => e.Color)
                .HasMaxLength(10)
                .HasDefaultValue("#000000");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("User_PK");

            entity.ToTable("User");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.PasswordHash).HasMaxLength(100);
            entity.Property(e => e.Username).HasMaxLength(100);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("User_RoleFK");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
