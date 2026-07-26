using CheckListItem.Domain.Entities;
using CleanMadeira.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WhiteLagoon.Domain.Entities;

namespace CleanMadeira.Infrastructure.Data;
public class ApplicationDbContext
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Utilizador> Utilizadores { get; set; }
    public DbSet<Company> Companies { get; set; }

    public DbSet<Property> Properties { get; set; }

    public DbSet<CleaningTask> CleaningTasks { get; set; }

    public DbSet<TaskPhoto> TaskPhotos { get; set; }

    public DbSet<InventoryItem> InventoryItems { get; set; }

    public DbSet<InventoryAlert> InventoryAlerts { get; set; }

    public DbSet<ChecklistItem> ChecklistItems { get; set; }

    public DbSet<CalendarIntegration> CalendarIntegrations { get; set; }

    public DbSet<Reservation> Reservations { get; set; }

    public DbSet<Maintenance> Maintenances { get; set; }

    public DbSet<MaintenanceProvider> MaintenanceProviders { get; set; }

    //public DbSet<Notification> Notifications { get; set; }

    public DbSet<AuditLog> AuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Property>()
            .HasMany(p => p.CleaningTasks)
            .WithOne(t => t.Property)
            .HasForeignKey(t => t.PropertyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<ApplicationUser>()
            .HasMany(u => u.CleaningTasks)
            .WithOne(t => t.AssignedUser)
            .HasForeignKey(t => t.AssignedUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<CleaningTask>()
            .HasMany(t => t.Photos)
            .WithOne(p => p.CleaningTask)
            .HasForeignKey(p => p.CleaningTaskId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<CleaningTask>()
            .HasMany(t => t.ChecklistItems)
            .WithOne(c => c.CleaningTask)
            .HasForeignKey(c => c.CleaningTaskId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Property>()
            .HasMany(p => p.InventoryItems)
            .WithOne(i => i.Property)
            .HasForeignKey(i => i.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Property>()
            .Property(p => p.Latitude)
            .HasPrecision(18, 6);

        builder.Entity<Property>()
            .Property(p => p.Longitude)
            .HasPrecision(18, 6);

        builder.Entity<ApplicationUser>()
            .Ignore(u => u.CleanerCode);

        builder.Entity<CalendarIntegration>()
            .HasOne(c => c.Property)
            .WithMany(p => p.CalendarIntegrations)
            .HasForeignKey(c => c.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Reservation>()
            .HasIndex(r => new
            {
                r.CalendarIntegrationId,
                r.ExternalUid
            })
            .IsUnique();

        builder.Entity<Reservation>()
            .HasOne(r => r.Property)
            .WithMany()
            .HasForeignKey(r => r.PropertyId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<Reservation>()
            .HasOne(r => r.CalendarIntegration)
            .WithMany()
            .HasForeignKey(r => r.CalendarIntegrationId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<Maintenance>(entity =>
        {
            entity.ToTable("Maintenances");

            entity.HasKey(m => m.Id);

            entity.Property(m => m.Title)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(m => m.Description)
                .HasMaxLength(2000);

            entity.Property(m => m.Priority)
                .IsRequired();

            entity.Property(m => m.Status)
                .IsRequired();

            entity.Property(m => m.ScheduledDate)
                .IsRequired();

            entity.Property(m => m.CreatedAt)
                .IsRequired();

            entity.HasOne(m => m.Property)
                .WithMany()
                .HasForeignKey(m => m.PropertyId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(m => m.AssignedUser)
                .WithMany()
                .HasForeignKey(m => m.AssignedUserId)
                .OnDelete(DeleteBehavior.SetNull);
            
           entity.HasOne(x => x.MaintenanceProvider)
                .WithMany()
                .HasForeignKey(x => x.MaintenanceProviderId)
                .OnDelete(DeleteBehavior.SetNull);
        });


        builder.Entity<MaintenanceProvider>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name)
                  .IsRequired()
                  .HasMaxLength(150);

            entity.Property(x => x.Category)
                  .HasMaxLength(100);

            entity.Property(x => x.Phone)
                  .HasMaxLength(30);

            entity.Property(x => x.Email)
                  .HasMaxLength(150);

            entity.HasOne(x => x.Owner)
                  .WithMany()
                  .HasForeignKey(x => x.OwnerId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
        /* builder.Entity<Property>()
             .HasMany(p => p.Reservations)
             .WithOne(r => r.Property)
             .HasForeignKey(r => r.PropertyId)
             .OnDelete(DeleteBehavior.Cascade);*/
    }
}