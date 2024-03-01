using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace UnitTestMVC.Web.Models;

public partial class TestContext : DbContext
{
   

    public TestContext(DbContextOptions<TestContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Product> Products { get; set; }
    public virtual DbSet<Category> Categories { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Product");

            entity.Property(e => e.Color)
                .HasMaxLength(50)
                .IsFixedLength();
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
        });
        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Category");
            //entity.HasData(new Category { Id=1,Name="Kalemler"},new Category { Id=2,Name="Defterler"});
       
            entity.Property(e => e.Name).HasMaxLength(200);
           
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
