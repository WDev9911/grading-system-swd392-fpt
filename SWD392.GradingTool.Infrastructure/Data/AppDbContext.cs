using Microsoft.EntityFrameworkCore;
using SWD392.GradingTool.Domain.Entities;

namespace SWD392.GradingTool.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<ClassGroup> ClassGroups { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Rubric> Rubrics { get; set; }
    public DbSet<Grade> Grades { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ─── ClassGroup ───────────────────────────────────────────────────
        modelBuilder.Entity<ClassGroup>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ClassName)
                  .IsRequired()
                  .HasMaxLength(100);
        });

        // ─── Student ──────────────────────────────────────────────────────
        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.StudentCode)
                  .IsRequired()
                  .HasMaxLength(20);

            entity.Property(e => e.FullName)
                  .IsRequired()
                  .HasMaxLength(200);

            // Index để tăng tốc truy vấn duplicate check
            entity.HasIndex(e => e.StudentCode)
                  .HasDatabaseName("IX_Students_StudentCode");

            // Relationship: Student belongs to ClassGroup
            entity.HasOne(e => e.ClassGroup)
                  .WithMany(e => e.Students)
                  .HasForeignKey(e => e.ClassGroupId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ─── Rubric ───────────────────────────────────────────────────────
        modelBuilder.Entity<Rubric>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.RubricName)
                  .IsRequired()
                  .HasMaxLength(200);

            entity.Property(e => e.MaxScore)
                  .IsRequired()
                  .HasColumnType("decimal(18,2)");

            entity.Property(e => e.Description)
                  .HasMaxLength(1000);

            // Index để kiểm tra duplicate RubricName nhanh hơn
            entity.HasIndex(e => e.RubricName)
                  .HasDatabaseName("IX_Rubrics_RubricName");
        });

        // ─── Grade ────────────────────────────────────────────────────────
        modelBuilder.Entity<Grade>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Score)
                  .IsRequired()
                  .HasColumnType("decimal(18,2)");

            entity.Property(e => e.Comment)
                  .HasMaxLength(1000);

            // Một Student và một Rubric chỉ có 1 điểm (Grade)
            entity.HasIndex(e => new { e.StudentId, e.RubricId })
                  .IsUnique()
                  .HasDatabaseName("IX_Grades_StudentId_RubricId");

            entity.HasOne(e => e.Student)
                  .WithMany() // Student entity doesn't have a Grades collection
                  .HasForeignKey(e => e.StudentId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Rubric)
                  .WithMany() // Rubric entity doesn't have a Grades collection
                  .HasForeignKey(e => e.RubricId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
