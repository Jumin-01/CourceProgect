using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace test2.Models;

public partial class HomeAccountingContext : DbContext
{
    public HomeAccountingContext()
    {
    }

    public HomeAccountingContext(DbContextOptions<HomeAccountingContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Credit> Credits { get; set; }

    public virtual DbSet<Creditpayment> Creditpayments { get; set; }

    public virtual DbSet<Plan> Plans { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Usersummary> Usersummaries { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;database=\"home accounting\";user=admin;password=admin", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.40-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("category");

            entity.HasIndex(e => e.ParentsCategory, "ParentsCategory");

            entity.Property(e => e.Name).HasMaxLength(30);

            entity.HasOne(d => d.ParentsCategoryNavigation).WithMany(p => p.InverseParentsCategoryNavigation)
                .HasForeignKey(d => d.ParentsCategory)
                .HasConstraintName("ParentsCategory");
        });

        modelBuilder.Entity<Credit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("credit");

            entity.HasIndex(e => e.UserId, "UserToCredit");

            entity.Property(e => e.Amount).HasPrecision(10, 2);
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.RemainingAmount).HasPrecision(10, 2);
            entity.Property(e => e.UserName).HasMaxLength(30);
        });

        modelBuilder.Entity<Creditpayment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("creditpayment");

            entity.HasIndex(e => e.CreditId, "idx_creditpayment_creditid");

            entity.HasIndex(e => e.UserName, "idx_creditpayment_userid");

            entity.Property(e => e.Amount).HasPrecision(10, 2);
            entity.Property(e => e.CreditAmount).HasPrecision(10, 2);
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.UserName).HasMaxLength(30);

            entity.HasOne(d => d.Credit).WithMany(p => p.Creditpayments)
                .HasForeignKey(d => d.CreditId)
                .HasConstraintName("Credit");
        });

        modelBuilder.Entity<Plan>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("plan");

            entity.HasIndex(e => e.CategoryId, "PlanCategory");

            entity.HasIndex(e => e.UserId, "idx_plan_userid");

            entity.Property(e => e.Amount).HasPrecision(10, 2);
            entity.Property(e => e.CategoryName).HasMaxLength(30);
            entity.Property(e => e.Type).HasColumnType("enum('income','expense')");
            entity.Property(e => e.UserName).HasMaxLength(30);

            entity.HasOne(d => d.Category).WithMany(p => p.Plans)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("PlanCategory");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("transaction");

            entity.HasIndex(e => e.CategoryId, "UserCategory");

            entity.HasIndex(e => e.UserId, "idx_transaction_userid");

            entity.Property(e => e.Amount).HasPrecision(10, 2);
            entity.Property(e => e.CategoryName).HasMaxLength(30);
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.Type).HasColumnType("enum('income','expense')");
            entity.Property(e => e.UserName).HasMaxLength(30);

            entity.HasOne(d => d.Category).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("UserCategory");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.Name })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("users");

            entity.HasIndex(e => e.Id, "Id");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Name).HasMaxLength(30);
            entity.Property(e => e.Balance).HasPrecision(10, 2);
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .UseCollation("utf8mb4_bin");
            entity.Property(e => e.Role).HasColumnType("enum('Parents','Children')");
        });

        modelBuilder.Entity<Usersummary>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("usersummary");

            entity.Property(e => e.TotalCreditAmount).HasPrecision(32, 2);
            entity.Property(e => e.TotalExpense).HasPrecision(32, 2);
            entity.Property(e => e.TotalIncome).HasPrecision(32, 2);
            entity.Property(e => e.TotalRemainingDebt).HasPrecision(32, 2);
            entity.Property(e => e.UserBalance).HasPrecision(10, 2);
            entity.Property(e => e.UserName).HasMaxLength(30);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
