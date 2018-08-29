using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace LMS.Models.LMSModels
{
    public partial class Team9Context : DbContext
    {
        public virtual DbSet<Administrator> Administrator { get; set; }
        public virtual DbSet<Assignment> Assignment { get; set; }
        public virtual DbSet<AssignmentCategory> AssignmentCategory { get; set; }
        public virtual DbSet<Class> Class { get; set; }
        public virtual DbSet<Course> Course { get; set; }
        public virtual DbSet<Department> Department { get; set; }
        public virtual DbSet<Enrolled> Enrolled { get; set; }
        public virtual DbSet<Professor> Professor { get; set; }
        public virtual DbSet<Student> Student { get; set; }
        public virtual DbSet<Submission> Submission { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseMySql("Server=atr.eng.utah.edu;User Id=u0228372;Password=changeme;Database=Team9");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Administrator>(entity =>
            {
                entity.Property(e => e.DOB).HasColumnType("datetime");

                entity.HasKey(e => e.UId);

                entity.Property(e => e.UId)
                    .HasColumnName("uID")
                    .HasMaxLength(8);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasColumnName("firstName")
                    .HasMaxLength(100);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasColumnName("lastName")
                    .HasMaxLength(100);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password")
                    .HasMaxLength(20);
            });

            modelBuilder.Entity<Assignment>(entity =>
            {
                entity.HasKey(e => e.AId);

                entity.HasIndex(e => e.Category)
                    .HasName("assCategory_idx");

                entity.Property(e => e.AId)
                    .HasColumnName("aID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Category)
                    .HasColumnName("category")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Contents)
                    .HasColumnName("contents")
                    .HasColumnType("varchar(8192)");

                entity.Property(e => e.DueDateTime)
                    .HasColumnName("dueDateTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.HandinType)
                    .HasColumnName("handinType")
                    .HasColumnType("tinyint(1)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(100);

                entity.Property(e => e.Points)
                    .HasColumnName("points")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.CategoryNavigation)
                    .WithMany(p => p.Assignment)
                    .HasForeignKey(d => d.Category)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("assCategory");
            });

            modelBuilder.Entity<AssignmentCategory>(entity =>
            {
                entity.HasKey(e => e.CatId);

                entity.HasIndex(e => e.ClassId)
                    .HasName("class_idx");

                entity.Property(e => e.CatId)
                    .HasColumnName("catID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ClassId)
                    .HasColumnName("classID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(100);

                entity.Property(e => e.Weight)
                    .HasColumnName("weight")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.AssignmentCategory)
                    .HasForeignKey(d => d.ClassId)
                    .HasConstraintName("cla");
            });

            modelBuilder.Entity<Class>(entity =>
            {
                entity.HasIndex(e => e.CatId)
                    .HasName("catID_idx");

                entity.HasIndex(e => e.Instructor)
                    .HasName("professor_idx");

                entity.Property(e => e.ClassId)
                    .HasColumnName("classID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CatId)
                    .HasColumnName("catID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.EndTime)
                    .HasColumnName("endTime")
                    .HasColumnType("time");

                entity.Property(e => e.Instructor)
                    .IsRequired()
                    .HasColumnName("instructor")
                    .HasMaxLength(8);

                entity.Property(e => e.Location)
                    .IsRequired()
                    .HasColumnName("location")
                    .HasMaxLength(100);

                entity.Property(e => e.Season)
                    .IsRequired()
                    .HasColumnName("season")
                    .HasMaxLength(10);

                entity.Property(e => e.StartTime)
                    .HasColumnName("startTime")
                    .HasColumnType("time");

                entity.Property(e => e.Year)
                    .HasColumnName("year")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Cat)
                    .WithMany(p => p.Class)
                    .HasForeignKey(d => d.CatId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("catID");

                entity.HasOne(d => d.InstructorNavigation)
                    .WithMany(p => p.Class)
                    .HasForeignKey(d => d.Instructor)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("professor");
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasKey(e => e.CatId);

                entity.HasIndex(e => e.DId)
                    .HasName("dID_index");

                entity.HasIndex(e => new { e.DId, e.CourseNum })
                    .HasName("dID_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.CatId)
                    .HasColumnName("catID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CourseNum)
                    .HasColumnName("courseNum")
                    .HasColumnType("int(11)");

                entity.Property(e => e.DId)
                    .HasColumnName("dID")
                    .HasMaxLength(4);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.D)
                    .WithMany(p => p.Course)
                    .HasForeignKey(d => d.DId)
                    .HasConstraintName("dID");
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasKey(e => e.Abbreviation);

                entity.Property(e => e.Abbreviation).HasMaxLength(4);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Enrolled>(entity =>
            {
                entity.HasKey(e => new { e.UId, e.ClassId });

                entity.HasIndex(e => e.ClassId)
                    .HasName("class_idx");

                entity.HasIndex(e => e.UId)
                    .HasName("student_idx");

                entity.Property(e => e.UId)
                    .HasColumnName("uID")
                    .HasMaxLength(8);

                entity.Property(e => e.ClassId)
                    .HasColumnName("classID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Grade)
                    .IsRequired()
                    .HasColumnName("grade")
                    .HasMaxLength(2);

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.Enrolled)
                    .HasForeignKey(d => d.ClassId)
                    .HasConstraintName("class");

                entity.HasOne(d => d.U)
                    .WithMany(p => p.Enrolled)
                    .HasForeignKey(d => d.UId)
                    .HasConstraintName("student");
            });

            modelBuilder.Entity<Professor>(entity =>
            {
                entity.Property(e => e.DOB).HasColumnType("datetime");
                entity.HasKey(e => e.UId);

                entity.HasIndex(e => e.Department)
                    .HasName("department_idx");

                entity.Property(e => e.UId)
                    .HasColumnName("uID")
                    .HasMaxLength(8);

                entity.Property(e => e.Department)
                    .IsRequired()
                    .HasColumnName("department")
                    .HasMaxLength(4);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasColumnName("firstName")
                    .HasMaxLength(100);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasColumnName("lastName")
                    .HasMaxLength(100);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password")
                    .HasMaxLength(20);

                entity.HasOne(d => d.DepartmentNavigation)
                    .WithMany(p => p.Professor)
                    .HasForeignKey(d => d.Department)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("department");
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.Property(e => e.DOB).HasColumnType("datetime");
                entity.HasKey(e => e.UId);

                entity.HasIndex(e => e.Major)
                    .HasName("major_idx");

                entity.Property(e => e.UId)
                    .HasColumnName("uID")
                    .HasMaxLength(8);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasColumnName("firstName")
                    .HasMaxLength(100);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasColumnName("lastName")
                    .HasMaxLength(100);

                entity.Property(e => e.Major)
                    .IsRequired()
                    .HasColumnName("major")
                    .HasMaxLength(4);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password")
                    .HasMaxLength(20);

                entity.HasOne(d => d.MajorNavigation)
                    .WithMany(p => p.Student)
                    .HasForeignKey(d => d.Major)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("major");
            });

            modelBuilder.Entity<Submission>(entity =>
            {
                entity.HasKey(e => new { e.UId, e.AId });

                entity.HasIndex(e => e.AId)
                    .HasName("aID_idx");

                entity.Property(e => e.UId)
                    .HasColumnName("uID")
                    .HasMaxLength(8);

                entity.Property(e => e.AId)
                    .HasColumnName("aID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Contents)
                    .IsRequired()
                    .HasColumnName("contents")
                    .HasColumnType("varchar(8192)");

                entity.Property(e => e.Score)
                    .HasColumnName("score")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Time)
                    .HasColumnName("time")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.A)
                    .WithMany(p => p.Submission)
                    .HasForeignKey(d => d.AId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("aID");

                entity.HasOne(d => d.U)
                    .WithMany(p => p.Submission)
                    .HasForeignKey(d => d.UId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("uID");
            });
        }
    }
}
