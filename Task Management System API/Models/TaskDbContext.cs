using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Task_Management_System_API.Models;

public partial class TaskDbContext : DbContext
{
    public TaskDbContext()
    {
    }

    public TaskDbContext(DbContextOptions<TaskDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<FileAttach> FileAttaches { get; set; }

    public virtual DbSet<Member> Members { get; set; }

    public virtual DbSet<MemberInvitation> MemberInvitations { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Task> Tasks { get; set; }

    public virtual DbSet<TaskAssign> TaskAssigns { get; set; }

    public virtual DbSet<TaskCategory> TaskCategories { get; set; }

    public virtual DbSet<TaskDetail> TaskDetails { get; set; }

    public virtual DbSet<UserDetail> UserDetails { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-MB7TAPL;Initial Catalog=TaskDB;User id=sa;Password=123; TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.CompanyId).HasName("PK__Company__2D971C4C2B8BA25A");

            entity.ToTable("Company");

            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CompanyName).HasMaxLength(50);
        });

        modelBuilder.Entity<FileAttach>(entity =>
        {
            entity.HasIndex(e => e.TaskDetailId, "IX_FileAttaches_TaskDetailId");

            entity.HasOne(d => d.TaskDetail).WithMany(p => p.FileAttaches).HasForeignKey(d => d.TaskDetailId);
        });

        modelBuilder.Entity<Member>(entity =>
        {
            entity.Property(e => e.MemberId).HasColumnName("MemberID");
        });

        modelBuilder.Entity<MemberInvitation>(entity =>
        {
            entity.HasKey(e => e.SendId);

            entity.HasIndex(e => e.TaskId, "IX_MemberInvitations_TaskID");

            entity.Property(e => e.TaskId).HasColumnName("TaskID");

            entity.HasOne(d => d.Task).WithMany(p => p.MemberInvitations).HasForeignKey(d => d.TaskId);
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.Property(e => e.NotificationEvent).IsRequired();
        });

        modelBuilder.Entity<Task>(entity =>
        {
            entity.HasIndex(e => e.TaskCategoryId, "IX_Tasks_TaskCategoryID");

            entity.Property(e => e.TaskId).HasColumnName("TaskID");
            entity.Property(e => e.CreateBy).IsRequired();
            entity.Property(e => e.Description).IsRequired();
            entity.Property(e => e.TaskCategoryId).HasColumnName("TaskCategoryID");
            entity.Property(e => e.UpdateBy).IsRequired();

            entity.HasOne(d => d.TaskCategory).WithMany(p => p.Tasks).HasForeignKey(d => d.TaskCategoryId);
        });

        modelBuilder.Entity<TaskAssign>(entity =>
        {
            entity.Property(e => e.TaskAssignId).HasColumnName("TaskAssignID");
            entity.Property(e => e.MemberId).HasColumnName("MemberID");
            entity.Property(e => e.TaskDetailId).HasColumnName("TaskDetailID");
            entity.Property(e => e.TaskId).HasColumnName("TaskID");

            entity.HasOne(d => d.Member).WithMany(p => p.TaskAssigns)
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.Task).WithMany(p => p.TaskAssigns)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TaskCategory>(entity =>
        {
            entity.Property(e => e.TaskCategoryId).HasColumnName("TaskCategoryID");
            entity.Property(e => e.CreateBy).IsRequired();
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.UpdateBy).IsRequired();
        });

        modelBuilder.Entity<TaskDetail>(entity =>
        {
            entity.HasIndex(e => e.TaskId, "IX_TaskDetails_TaskID");

            entity.Property(e => e.TaskDetailId).HasColumnName("TaskDetailID");
            entity.Property(e => e.CompleteBy).IsRequired();
            entity.Property(e => e.CreateBy).IsRequired();
            entity.Property(e => e.Description).IsRequired();
            entity.Property(e => e.TaskId).HasColumnName("TaskID");
            entity.Property(e => e.UpdateBy).IsRequired();

            entity.HasOne(d => d.Task).WithMany(p => p.TaskDetails).HasForeignKey(d => d.TaskId);
        });

        modelBuilder.Entity<UserDetail>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__UserDeta__1788CC4C01A13DE7");

            entity.ToTable("UserDetail");

            entity.Property(e => e.PassWord).HasMaxLength(100);
            entity.Property(e => e.RoleId).HasMaxLength(255);
            entity.Property(e => e.UserName)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.Member).WithMany(p => p.UserDetails)
                .HasForeignKey(d => d.MemberId)
                .HasConstraintName("FK_UserDetail_Member");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("UserRole");

            entity.HasIndex(e => e.RoleId, "UQ__UserRole__8AFACE1B63D11A08").IsUnique();

            entity.Property(e => e.RoleId)
                .HasMaxLength(36)
                .HasDefaultValueSql("(newid())");
            entity.Property(e => e.RoleName).HasMaxLength(200);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
