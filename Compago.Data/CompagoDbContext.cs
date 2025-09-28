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

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("User_PK");

            entity.ToTable("User");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.PasswordHash).HasMaxLength(100);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Username).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
